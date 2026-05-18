import { Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { Notification } from "../models/notification.model";

@Injectable({providedIn: 'root'}) 
export class NotificationService {
    private apiUrl = `${environment.apiUrl}/notifications`;

    constructor(private http: HttpClient) {}

    getUnreadCount(): Observable<{ unreadCount: number}> {
        return this.http.get<{ unreadCount: number }>(`${this.apiUrl}/unread-count`);
    }

    markAsRead(id: string): Observable<void> {
        return this.http.put<void>(`${this.apiUrl}/${id}/read`, {});
    }

    getMyNotifications(): Observable<Notification[]> {
        return this.http.get<Notification[]>(this.apiUrl);
    }

    markAllAsRead(): Observable<void> {
        return this.http.put<void>(`${this.apiUrl}/read-all`,{});
    }

    deleteNotification(id: string): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }
}