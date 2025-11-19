import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SecurityContext, SimpleChanges } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';
import { Comment } from '../../Models/Comment';

@Component({
  standalone: true,
  selector: 'app-reply-comment',
  templateUrl: './reply-comment.html',
  imports: [ReactiveFormsModule, CommonModule]
})
export class ReplyCommentComponent implements OnInit, OnChanges{
  @Input() comment?: Comment
  @Output() replyAdded = new EventEmitter<void>();
  safeContent!: string | null;

  constructor(private sanitizer: DomSanitizer) { }

  ngOnInit() {
    this.setSanitizedString(this.comment!.content);
  }
  ngOnChanges(changes: SimpleChanges): void {
      let changedComment:Comment = changes['comment'].currentValue;
      this.safeContent = this.sanitizer.sanitize(SecurityContext.HTML, changedComment.content);
  }

  private setSanitizedString(content:string){
    this.safeContent = this.sanitizer.sanitize(SecurityContext.HTML, content);
  }
}
