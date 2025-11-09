import { Routes } from '@angular/router';
import { CommentListComponent } from './Components/comment-list/comment-list';
import { SelectedCommentComponent } from './Components/selected-comment/selected-comment';
import { ReplyCommentComponent } from './Components/reply-comment/reply-comment';

export const routes: Routes = [
  { path: '', component: CommentListComponent },
  { path: 'comments/:id', component: SelectedCommentComponent },
  { path: 'comments/:id', component: ReplyCommentComponent },
  { path: '**', redirectTo: '' }
];
