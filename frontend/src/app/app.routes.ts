import { Routes } from '@angular/router';
import { CommentFormComponent } from './Components/comment-form/comment-form';
import { CommentListComponent } from './Components/comment-list/comment-list';

export const routes: Routes = [
  { path: '', component: CommentListComponent },
  { path: 'new', component: CommentFormComponent },
  { path: '**', redirectTo: '' }
];
