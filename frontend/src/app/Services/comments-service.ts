import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { Comment } from '../Models/Comment';
import { SelectedComment } from '../Models/SelectedComment';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CommentsService {
  private baseUrl = environment.apiBaseUrl;
  private apiUrl = `${this.baseUrl}/api/comment`;

  constructor(private http: HttpClient) { }

  getComments(page = 1, orderBy = 'createdAt', order = 'desc'): Observable<any> {
    const params = new HttpParams()
      .set('page', page)
      .set('orderBy', orderBy)
      .set('order', order);
    return this.http.get<any>(this.apiUrl, { params });
  }

  addComment(formData: FormData): Observable<Comment> {
    return this.http.post<Comment>(this.apiUrl, formData);
  }

  addFile(formFile: File, commentId: string): Observable<any> {
    const formData = new FormData();
    formData.append('formFile', formFile, formFile.name);
    return this.http.post<File>(`${this.baseUrl}/api/files/${commentId}`, formData);
  }

  private downloadFile(commentId: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/api/files/${commentId}`, { responseType: 'blob' });
  }
  download(commentId: string) {
    this.downloadFile(commentId).subscribe(blob => {
      const a = document.createElement('a');
      const url = URL.createObjectURL(blob);
      a.href = url;
      a.download = `comment_${commentId}`;
      a.click();
      URL.revokeObjectURL(url);
    });
  }

  getCommentById(id: string): Observable<SelectedComment> {
    return this.http.get<SelectedComment>(`${this.apiUrl}/${id}`);
  }

  getReplies(parentId: string): Observable<Comment[]> {
    return this.http.get<Comment[]>(`${this.apiUrl}/${parentId}/children`);
  }

  selectedComment = signal<Comment | null>(null);
}
