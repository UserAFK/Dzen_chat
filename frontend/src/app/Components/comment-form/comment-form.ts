import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommentsService } from '../../Services/comments-service';
import { CommonModule } from '@angular/common';
import { SignalrService } from '../../Services/signalr-service';
import { Comment } from '../../Models/Comment';
import { RecaptchaModule, RecaptchaComponent } from 'ng-recaptcha';

@Component({
  standalone: true,
  selector: 'app-comment-form',
  imports: [ReactiveFormsModule, CommonModule, RecaptchaModule],
  templateUrl: './comment-form.html'
})
export class CommentFormComponent implements OnInit, AfterViewInit {
  @Input() parentCommentId?: string;
  @Output() commentAdded = new EventEmitter<void>();
  @ViewChild('captchaRef', { static: false }) captchaRef!: ElementRef;
  @ViewChild('contentArea') contentArea!: ElementRef<HTMLTextAreaElement>;
  private siteKey = '6Ld9hAwsAAAAACbiIpgrihTximVSIxCsUYNveDYh';
  selectedFile?: File;
  form!: FormGroup;
  private recaptchaWidgetId: number | undefined;

  constructor(private fb: FormBuilder, private service: CommentsService, private signalrService: SignalrService) { }
  ngOnInit(): void {
    this.form = this.fb.group({
      username: ['', [Validators.required, Validators.pattern(/^[A-Za-z0-9]+$/)]],
      email: ['', [Validators.required, Validators.email]],
      homepage: [''],
      content: ['', Validators.required],
      parentCommentId: [this.parentCommentId ?? null],
      recaptcha: ['', Validators.required]
    });
  }

  ngAfterViewInit(): void {
    this.initRecaptcha();
  }

  initRecaptcha() {
    const interval = setInterval(() => {
      if (window.grecaptcha?.enterprise) {
        clearInterval(interval);

        this.recaptchaWidgetId = window.grecaptcha.enterprise.render(
          this.captchaRef.nativeElement,
          {
            sitekey: this.siteKey,
            callback: (token: string) => this.onCaptchaResolved(token)
          }
        );
      }
    }, 200);
  }

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }

  applyTag(tag: string) {
    const textarea = this.contentArea.nativeElement;
    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const currentContent = textarea.value;

    if (start >= end) return;

    const selectedText = currentContent.substring(start, end);

    const openTag = tag === 'a' ? `<${tag} href="${selectedText}" title="${selectedText}">` : `<${tag}>`;
    const closeTag = `</${tag}>`;

    const hasOpen = currentContent.substring(start - openTag.length, start) === openTag;
    const hasClose = currentContent.substring(end, end + closeTag.length) === closeTag;

    let newContent: string;

    if (hasOpen && hasClose && tag !== 'a') {
      newContent =
        currentContent.substring(0, start - openTag.length) +
        selectedText +
        currentContent.substring(end + closeTag.length);
    } else {
      newContent =
        currentContent.substring(0, start) +
        openTag +
        selectedText +
        closeTag +
        currentContent.substring(end);
    }

    this.form.get('content')!.setValue(newContent);

    setTimeout(() => {
      textarea.focus();
      textarea.selectionStart = start + openTag.length;
      textarea.selectionEnd = end + openTag.length;
    }, 0);
  }

  onCaptchaResolved(token: Event | string | null) {
    if (token) {
      this.form.get('recaptcha')?.setValue(token);
      this.form.get('recaptcha')?.updateValueAndValidity();
    } else {
      this.form.get('recaptcha')?.reset();
    }
  }

  submit() {
    if (this.form!.invalid) return;
    if (this.hasUnclosedTags(this.form!.get('content')?.value)) {
      window.alert('Input has unclosed tags.')
      return;
    }
    const newId = crypto.randomUUID();
    let newComment = this.form.getRawValue() as Comment;
    newComment.id = newId;
    newComment.parentCommentId = this.parentCommentId;
    const formData = new FormData();
    Object.entries(this.form!.value).forEach(([key, val]) => {
      if (val !== null && val !== undefined) formData.append(key, val.toString());
    });
    formData.append("id", newId);
    if (this.selectedFile) formData.append('file', this.selectedFile);

    switch (this.parentCommentId) {
      case undefined:
        this.service.addComment(formData).subscribe({
          next: (c) => this.onCommentSent(),
          error: (e) => window.alert(e.error.detail)
        });
        break;
      default:
        this.signalrService.sendReply(newComment);
        if (this.selectedFile) this.service.addFile(this.selectedFile, newId).subscribe();
        this.onCommentSent();
        break;
    }
  }

  private onCommentSent() {
    this.form!.reset();
    this.selectedFile = undefined;
    if (this.recaptchaWidgetId !== undefined && window.grecaptcha?.enterprise) {
      window.grecaptcha.enterprise.reset(this.recaptchaWidgetId);
    }
    this.commentAdded.emit();
  }

  private hasUnclosedTags(value: string): boolean {
    if (!value) return false;
    const tags = ['i', 'strong', 'code', 'a'];
    const unclosed: string[] = [];

    for (const tag of tags) {
      const openCount = (value.match(new RegExp(`<${tag}[^>]*>`, 'g')) || []).length;
      const closeCount = (value.match(new RegExp(`</${tag}>`, 'g')) || []).length;

      if (openCount > closeCount) {
        unclosed.push(tag);
      }
    }

    if (unclosed.length > 0) {
      console.warn('Unclosed tags:', unclosed.join(', '));
      return true;
    }
    return false;
  }
}
