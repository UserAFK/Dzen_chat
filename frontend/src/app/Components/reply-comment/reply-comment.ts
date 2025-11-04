import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Comment } from '../../Models/Comment';

@Component({
  standalone: true,
  selector: 'app-reply-comment',
  templateUrl: './reply-comment.html',
  imports: [ReactiveFormsModule, CommonModule]
})
export class ReplyCommentComponent {
  @Input() parentComment?: Comment
  @Output() replyAdded = new EventEmitter<void>();

  constructor() { }

}
