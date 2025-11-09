import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output, SecurityContext } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';
import { Comment } from '../../Models/Comment';

@Component({
  standalone: true,
  selector: 'app-reply-comment',
  templateUrl: './reply-comment.html',
  imports: [ReactiveFormsModule, CommonModule]
})
export class ReplyCommentComponent implements OnInit {
  @Input() comment?: Comment
  @Output() replyAdded = new EventEmitter<void>();
  safeContent!: string | null;

  constructor(private sanitizer: DomSanitizer) { }

  ngOnInit() {
    this.safeContent = this.sanitizer.sanitize(SecurityContext.HTML, this.comment!.content);
  }
}
