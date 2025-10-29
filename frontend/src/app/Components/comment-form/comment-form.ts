// src/app/components/comment-form/comment-form.component.ts
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommentsService } from '../../Services/comments-service';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-comment-form',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './comment-form.html'
})
export class CommentFormComponent implements OnInit{
  @Output() commentAdded = new EventEmitter<void>();
  selectedFile?: File;
  form!: FormGroup;

  

  
  constructor(private fb: FormBuilder, private service: CommentsService) {}
  ngOnInit(): void {
    this.form = this.fb.group({
    username: ['', [Validators.required, Validators.pattern(/^[A-Za-z0-9]+$/)]],
    email: ['', [Validators.required, Validators.email]],
    homepage: [''],
    content: ['', Validators.required],
    parentCommentId: [null]
    });
  }

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }

  submit() {
    if (this.form!.invalid) return;

    const formData = new FormData();
    Object.entries(this.form!.value).forEach(([key, val]) => {
      if (val !== null && val !== undefined) formData.append(key, val.toString());
    });
    if (this.selectedFile) formData.append('file', this.selectedFile);

    this.service.addComment(formData).subscribe({
      next: () => {
        this.form!.reset();
        this.commentAdded.emit();
      }
    });
  }
}
