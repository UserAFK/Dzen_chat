import { Comment } from './Comment';

export interface SelectedComment extends Comment{  
  parentComment?:Comment;
  replies?:Comment[];
}