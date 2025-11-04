import { Component,  OnInit, signal } from '@angular/core';
import { CommentsService } from '../../Services/comments-service';
import { Comment } from '../../Models/Comment';
import { CommonModule, DatePipe } from '@angular/common'
import { CommentFormComponent } from '../comment-form/comment-form';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-comment-list',
  templateUrl: './comment-list.html',
  imports: [DatePipe, CommentFormComponent, CommonModule]
})
export class CommentListComponent implements OnInit {
  comments$!: Observable<Comment[]>;
  page = 1;
  orderBy = 'createdAt';
  order = 'desc';

  constructor(private service: CommentsService, private router: Router) { }

  ngOnInit() {
    this.load();
  }

  load() {
    this.comments$ = this.service.getComments(this.page, this.orderBy, this.order);

  }

  onCommentAdded() {
    this.load();
    this.closeForm();
  }
  
  showForm = signal<boolean>(false);
  
  openForm(): void {
    this.showForm.set(true);
  }

  /**
   * Closes the comment form modal.
   */
  closeForm(): void {
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
