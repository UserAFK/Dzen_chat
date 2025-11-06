import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommentsService } from '../../Services/comments-service';
import { CommonModule } from '@angular/common';
import { SignalrService } from '../../Services/signalr-service';
import { Comment } from '../../Models/Comment';

@Component({
  standalone: true,
  selector: 'app-comment-form',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './comment-form.html'
})
export class CommentFormComponent implements OnInit {
  @Input() parentCommentId?: string;
  @Output() commentAdded = new EventEmitter<void>();
  selectedFile?: File;
  form!: FormGroup;

  @ViewChild('contentArea') contentArea!: ElementRef<HTMLTextAreaElement>;

  constructor(private fb: FormBuilder, private service: CommentsService, private signalrService: SignalrService) { }
  ngOnInit(): void {
    this.form = this.fb.group({
      username: ['', [Validators.required, Validators.pattern(/^[A-Za-z0-9]+$/)]],
      email: ['', [Validators.required, Validators.email]],
      homepage: [''],
      content: ['', Validators.required],
      parentCommentId: [this.parentCommentId ?? null]
    });
  }

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }
  
  applyItalics(): void {
    const textarea = this.contentArea.nativeElement;
    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const currentContent = textarea.value;

    if (start < end) {
      const selectedText = currentContent.substring(start, end);
      if (currentContent.substring(start - 3, start) === '<i>' && currentContent.substring(end, end + 4) === '</i>') {

        const newContent =
          currentContent.substring(0, start - 3) +
          selectedText +
          currentContent.substring(end + 4);

        this.form.get('content')!.setValue(newContent);
      } else {
        const newContent =
          currentContent.substring(0, start) +
          `<i>${selectedText}</i>` +
          currentContent.substring(end);

        this.form.get('content')!.setValue(newContent);
      }
      setTimeout(() => {
        textarea.focus();
        textarea.selectionStart = start + 3;
        textarea.selectionEnd = end + 3;
      }, 0);
    }
  }

  submit() {
    if (this.form!.invalid) return;

    const formData = new FormData();
    Object.entries(this.form!.value).forEach(([key, val]) => {
      if (val !== null && val !== undefined) formData.append(key, val.toString());
    });
    if (this.selectedFile) formData.append('file', this.selectedFile);

    switch (this.parentCommentId) {
      case undefined:
        this.service.addComment(formData).subscribe();
        break;
      default:
        this.signalrService.sendReply(this.form.getRawValue() as Comment);
        break;
    }
    this.form!.reset();
    this.commentAdded.emit();
  }
}
