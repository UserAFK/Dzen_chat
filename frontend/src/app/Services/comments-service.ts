import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Comment } from '../Models/Comment';

@Injectable({ providedIn: 'root' })
export class CommentsService {
  private baseUrl = 'https://localhost:7242'
  private apiUrl = `${this.baseUrl}/api/comment`;

  constructor(private http: HttpClient) {}

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

  downloadFile(commentId: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/${commentId}/file`, { responseType: 'blob' });
  }
}
