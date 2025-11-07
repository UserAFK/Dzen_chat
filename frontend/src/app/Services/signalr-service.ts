import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Comment } from '../Models/Comment';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  private readonly hubConnection: HubConnection;
  private basePath = environment.basePath;
  constructor() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${this.basePath}/commentHub`)
      .build();
  }

  getHubConnection() {
    return this.hubConnection;
  }

  async connect() {
    if (this.hubConnection.connectionId) return;
    try {
      await this.hubConnection.start();
    } catch (error) {
      console.error('SignalR connection error', error)
    }
  }

  async disconect(id: string) {
    this.hubConnection.invoke('LeaveCommentGroup', id)
    await this.hubConnection.stop();
  }

  leaveGroup(id: string) {
    this.hubConnection.invoke('LeaveCommentGroup', id)
  }

  async sendReply(comment: Comment) {
    await this.hubConnection.invoke('SendReply', comment);
  }

  async joinCommentGroup(id: string) {
    await this.hubConnection.invoke('JoinCommentGroup', id)
  }
}
