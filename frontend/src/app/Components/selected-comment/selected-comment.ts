import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommentsService } from '../../Services/comments-service';
import { CommonModule, DatePipe, Location } from '@angular/common';
import { ReplyCommentComponent } from '../reply-comment/reply-comment';
import { CommentFormComponent } from '../comment-form/comment-form';
import { SelectedComment } from '../../Models/SelectedComment';
import { Observable } from 'rxjs';
import { Comment } from '../../Models/Comment'

@Component({
  standalone: true,
  selector: 'app-selected-comment',
  templateUrl: './selected-comment.html',
  imports: [CommonModule, DatePipe, CommentFormComponent, ReplyCommentComponent]
})
export class SelectedCommentComponent implements OnInit {
  comment$?: Observable<SelectedComment>;
  private id!: string;

  constructor(private route: ActivatedRoute, private service: CommentsService,private location: Location) { }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.id = params.get('id') ?? 'error';
      this.loadComment();
    });
  }

  loadComment() {
    this.comment$ = this.service.getCommentById(this.id);
  }
  download(comment: Comment) {
    this.service.download(comment.id);
  }
  onReplyAdded() {
    this.loadComment();
  }
  back() {
    this.location.back();
  }
}
