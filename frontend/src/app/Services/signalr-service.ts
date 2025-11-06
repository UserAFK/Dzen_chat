import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Comment } from '../Models/Comment';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  private readonly hubConnection: HubConnection;

  constructor() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('https://localhost:7242/commentHub')
      .build();
  }

  getHubConnection() {
    return this.hubConnection;
  }
  async test() {
    this.hubConnection.invoke('Test', 'hello')
      .catch(err => console.error('Test failed:', err));

  }

  async connect() {
    try {
      await this.hubConnection.start();
    } catch (error) {
      console.error('SignalR connection error', error)
    }
  }

  async disconect(id:string) {
    this.hubConnection.invoke('LeaveCommentGroup', id)
    await this.hubConnection.stop();
  }

  async sendReply(comment: Comment) {
    await this.hubConnection.invoke('SendReply', comment);
  }

  async joinCommentGroup(id: string) {
    await this.hubConnection.invoke('JoinCommentGroup', id)
  }
}
