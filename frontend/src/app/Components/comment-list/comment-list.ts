import { Component, inject, Injector, OnInit, runInInjectionContext } from '@angular/core';
import { CommentsService } from '../../Services/comments-service';
import { Comment } from '../../Models/Comment';
import { CommonModule, DatePipe } from '@angular/common'
import { CommentFormComponent } from '../comment-form/comment-form';
import { Observable } from 'rxjs';

@Component({
  standalone: true,
  selector: 'app-comment-list',
  templateUrl: './comment-list.html',
  imports:[ DatePipe, CommentFormComponent ,CommonModule]
})
export class CommentListComponent implements OnInit {
  // comments$!: Observable< {'comments':Comment[]}>;
  comments$!: Observable< Comment[]>;
  page = 1;
  orderBy = 'createdAt';
  order = 'desc';

  constructor(private service: CommentsService) {}

  ngOnInit() {
    this.load();
  }

  load() {  
    this.comments$ = this.service.getComments(this.page, this.orderBy, this.order);
    
  }

  onCommentAdded() {
    this.load();
  }

  download(comment: Comment) {
    this.service.downloadFile(comment.id).subscribe(blob => {
      const a = document.createElement('a');
      const url = URL.createObjectURL(blob);
      a.href = url;
      a.download = `comment_${comment.id}`;
      a.click();
      URL.revokeObjectURL(url);
    });
  }

  toggleSort(field: string) {
    this.order = this.order === 'asc' ? 'desc' : 'asc';
    this.orderBy = field;
    this.load();
  }
}
