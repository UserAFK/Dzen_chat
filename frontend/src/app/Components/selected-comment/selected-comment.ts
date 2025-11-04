import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommentsService } from '../../Services/comments-service';
import { CommonModule, DatePipe, Location } from '@angular/common';
import { ReplyCommentComponent } from '../reply-comment/reply-comment';
import { CommentFormComponent } from '../comment-form/comment-form';
import { SelectedComment } from '../../Models/SelectedComment';
import { merge, Observable, startWith, Subject, switchMap, tap } from 'rxjs';
import { Comment } from '../../Models/Comment'

@Component({
  standalone: true,
  selector: 'app-selected-comment',
  templateUrl: './selected-comment.html',
  imports: [CommonModule, DatePipe, CommentFormComponent, ReplyCommentComponent]
})
export class SelectedCommentComponent implements OnInit {
  comment$!: Observable<SelectedComment>;
  private id!: string;
  private reloadTrigger = new Subject<void>();

  constructor(private route: ActivatedRoute, private service: CommentsService,private location: Location) { }

  ngOnInit() {
    const idStream = this.route.paramMap.pipe(
      tap(params => this.id = params.get('id') ?? 'error')
    );
    const triggerStream = merge(
      idStream,
      this.reloadTrigger.asObservable()
    ).pipe(
      startWith(null)
    );
    this.comment$ = triggerStream.pipe(
      switchMap(() => this.service.getCommentById(this.id))
    );
  }

  download(comment: Comment) {
    this.service.download(comment.id);
  }
  onReplyAdded() {
    this.reloadTrigger.next();
  }
  back() {
    this.location.back();
  }
}
