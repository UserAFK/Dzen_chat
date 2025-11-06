import { Component, OnInit, signal } from '@angular/core';
import { CommentsService } from '../../Services/comments-service';
import { Comment } from '../../Models/Comment';
import { CommonModule, DatePipe } from '@angular/common'
import { CommentFormComponent } from '../comment-form/comment-form';
import { BehaviorSubject, Observable } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-comment-list',
  templateUrl: './comment-list.html',
  imports: [DatePipe, CommentFormComponent, CommonModule]
})
export class CommentListComponent implements OnInit {
  comments$: BehaviorSubject<Comment[]> = new BehaviorSubject<Comment[]>([]);
  page = 1;
  orderBy = 'createdAt';
  order = 'desc';

  constructor(private service: CommentsService, private router: Router) { }

  ngOnInit() {
    this.load();
  }

  load() {
    this.service.getComments(this.page, this.orderBy, this.order)
      .subscribe(data => this.comments$.next(data))
  }

  onCommentAdded() {
    this.load();
    this.closeForm();
  }

  showForm = signal<boolean>(false);

  openForm(): void {
    this.showForm.set(true);
  }

  closeForm(): void {
    this.load();
    this.showForm.set(false);
  }

  download(comment: Comment) {
    this.service.download(comment.id);
  }

  toggleSort(field: string) {
    this.order = this.order === 'asc' ? 'desc' : 'asc';
    this.orderBy = field;
    this.load();
  }
  
  goToComment(id: string) {
    this.router.navigate(['/comments', id]);
  }
}
