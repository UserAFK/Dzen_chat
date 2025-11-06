import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommentsService } from '../../Services/comments-service';
import { CommonModule, DatePipe, Location } from '@angular/common';
import { ReplyCommentComponent } from '../reply-comment/reply-comment';
import { CommentFormComponent } from '../comment-form/comment-form';
import { SelectedComment } from '../../Models/SelectedComment';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { Comment } from '../../Models/Comment'
import { SignalrService } from '../../Services/signalr-service';

@Component({
  standalone: true,
  selector: 'app-selected-comment',
  templateUrl: './selected-comment.html',
  imports: [CommonModule, RouterModule, DatePipe, CommentFormComponent, ReplyCommentComponent]
})
export class SelectedCommentComponent implements OnInit, OnDestroy {
  isLoaded = false;
  isLoadedSUB$: Subject<boolean> = new Subject();
  //comment!: SelectedComment;
  comment$: BehaviorSubject<SelectedComment | null> = new BehaviorSubject<SelectedComment | null>(null);;
  private id!: string;

  constructor(private route: ActivatedRoute,
    private service: CommentsService,
    private signalrService: SignalrService,
    private location: Location) { }

  ngOnInit() {
    this.isLoadedSUB$.next(false);
    this.route.paramMap.subscribe(params => {
      this.id = params.get('id') ?? 'error';
      this.connectSignalR();
    });
  }
  ngOnDestroy(): void {
    this.signalrService.disconect(this.id);
  }
  download(comment: Comment) {
    this.service.download(comment.id);
  }
  private connectSignalR() {
    this.signalrService.connect().then(() => {
      this.signalrService.getHubConnection().on('ReceiveComment',
        (comment: SelectedComment) => {
          this.comment$.next(comment)
          //this.comment = comment;
          this.isLoaded = true;
          this.isLoadedSUB$.next(true);
        });

      this.signalrService.getHubConnection().on('JoinedCommentGroup',
        (comment: SelectedComment) => {
          this.comment$.next(comment)
          //this.comment = comment;
          this.isLoaded = true;
          this.isLoadedSUB$.next(true);
          //this.comment$.next(comment);
        })

      this.signalrService.joinCommentGroup(this.id);
    }).catch(err => console.error("connectSignalR error:", err));
  }
  join() {
    this.signalrService.joinCommentGroup(this.id);
  }
}
