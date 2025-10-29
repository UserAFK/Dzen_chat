export interface Comment{
  id: string;
  parentCommentId?: string;
  username: string;
  email: string;
  homepage?: string;
  content: string;
  createdAt: string;
  fileType?: string;
}