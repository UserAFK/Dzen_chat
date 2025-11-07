import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommentsService } from '../../Services/comments-service';
import { CommonModule } from '@angular/common';
import { SignalrService } from '../../Services/signalr-service';
import { Comment } from '../../Models/Comment';
import { RecaptchaModule } from 'ng-recaptcha';

@Component({
  standalone: true,
  selector: 'app-comment-form',
  imports: [ReactiveFormsModule, CommonModule, RecaptchaModule],
  templateUrl: './comment-form.html'
})
export class CommentFormComponent implements OnInit {
  @Input() parentCommentId?: string;
  @Output() commentAdded = new EventEmitter<void>();
  selectedFile?: File;
  form!: FormGroup;
  siteKey: string = '6LdnUwQsAAAAADFzg6fX9dzRzdXz6L80h_zwOZVb';

  @ViewChild('contentArea') contentArea!: ElementRef<HTMLTextAreaElement>;

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


  onCaptchaResolved(token: string | null) {
    if (token) {
      this.form.get('recaptcha')?.setValue(token);
      this.form.get('recaptcha')?.updateValueAndValidity();
    } else {
      this.form.get('recaptcha')?.reset();
    }
  }

  submit() {
    if (this.form!.invalid) return;

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
        this.service.addComment(formData).subscribe();
        break;
      default:
        this.signalrService.sendReply(newComment);
        if (this.selectedFile) this.service.addFile(this.selectedFile, newId).subscribe();
        break;
    }
    this.form!.reset();
    this.selectedFile = undefined;
    this.commentAdded.emit();
  }
}
