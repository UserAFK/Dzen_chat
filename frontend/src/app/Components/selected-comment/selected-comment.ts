import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommentsService } from '../../Services/comments-service';
import { CommonModule, DatePipe } from '@angular/common';
import { ReplyCommentComponent } from '../reply-comment/reply-comment';
import { CommentFormComponent } from '../comment-form/comment-form';
import { SelectedComment } from '../../Models/SelectedComment';
import { BehaviorSubject } from 'rxjs';
import { Comment } from '../../Models/Comment'
import { SignalrService } from '../../Services/signalr-service';

@Component({
  standalone: true,
  selector: 'app-selected-comment',
  templateUrl: './selected-comment.html',
  imports: [CommonModule, RouterModule, DatePipe, CommentFormComponent, ReplyCommentComponent]
})
export class SelectedCommentComponent implements OnInit, OnDestroy {
  isLoaded = signal<boolean>(false);
  comment$: BehaviorSubject<SelectedComment | null> = new BehaviorSubject<SelectedComment | null>(null);
  private id!: string;

  constructor(private route: ActivatedRoute,
    private service: CommentsService,
    private signalrService: SignalrService,
    private router: Router) { }

  ngOnInit() {
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
  goToComment(id:string){
    this.router.navigate(['/comments', id]);
    this.leave(this.id)
    this.join(id)
    this.id = id;
  }
  join(id:string) {
    this.signalrService.joinCommentGroup(id);
  }
  leave(id:string) {
    this.signalrService.leaveGroup(id);
  }

  private connectSignalR() {
    this.signalrService.connect().then(() => {
      this.signalrService.getHubConnection().on('ReceiveComment',
        (comment: SelectedComment) => {
          this.comment$.next(comment)
          this.isLoaded.set(true);
        });

      this.signalrService.getHubConnection().on('JoinedCommentGroup',
        (comment: SelectedComment) => {
          this.comment$.next(comment)
          this.isLoaded.set(true);
        })

      this.signalrService.joinCommentGroup(this.id);
    }).catch(err => console.error("connectSignalR error:", err));
  }  
}
