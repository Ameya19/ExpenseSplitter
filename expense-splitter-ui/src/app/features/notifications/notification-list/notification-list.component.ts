import { Component, OnInit } from "@angular/core";
import { NotificationService } from "../../../core/services/notification.service";
import { Notification } from "../../../core/models/notification.model";
import { MatButton, MatIconButton } from "@angular/material/button";
import { MatIcon } from "@angular/material/icon";
import { CommonModule } from "@angular/common";
import { MatProgressSpinner } from "@angular/material/progress-spinner";
import { MatCard, MatCardContent } from "@angular/material/card";

@Component({
    selector: 'app-notification-list',
    templateUrl: 'notification-list.component.html',
    styleUrl: 'notification-list.component.scss',
    imports: [MatButton, MatIcon, CommonModule, MatProgressSpinner, MatCard, MatCardContent, MatIconButton]
})
export class NotificationListComponent implements OnInit {
    isLoading = false;
    unreadCount = 0;
    notifications: Notification[] = [];

    constructor(private notificationService: NotificationService) {}

    ngOnInit(): void {
        this.loadNotifications();
    }

    loadNotifications(): void {
        this.isLoading = true;
        this.notificationService.getMyNotifications().subscribe({
            next: (notifications) => {
                this.notifications = notifications;
                this.isLoading = false;
            },
            error: () => {
                this.isLoading = false;
            }
        })
    }

    markAllAsRead(): void {
        this.notificationService.markAllAsRead().subscribe({
            next: () => {
                this.notifications.forEach(n => n.isRead === true);
                this.unreadCount = 0;
            }
        });
    }

    markAsRead(id: string): void {
        this.notificationService.markAsRead(id). subscribe({
            next: () => {
                const notification = this.notifications.find(m => m.id === id);
                if(notification)
                {
                    notification.isRead = true;
                    this.unreadCount = this.notifications.filter(m => !m.isRead).length;
                }
            }
        });
    }

    deleteNotification(id: string) : void{
        this.notificationService.deleteNotification(id).subscribe({
            next: () => {
                this.notifications = this.notifications.filter(m => m.id !== id);
                this.unreadCount = this.notifications.filter(m => m.isRead === false).length;
            },
            error: () => {}
        });
    }

    getNotificationIcon(type: string) : string {
        const icons: Record<string, string> = {
            'ExpenseAdded':          'receipt_long',
            'ExpenseUpdated':        'edit',
            'ExpenseDeleted':        'delete',
            'SettlementRequested':   'payments',
            'SettlementCompleted':   'check_circle',
            'GroupInvite':           'group_add',
            'MemberJoined':          'person_add'
        };
        return icons[type] || 'notifications';
    }

    getNotificationColor(type: string): string {
        const colors: Record<string, string> = {
          'ExpenseAdded':          '#4caf50',
          'ExpenseUpdated':        '#2196f3',
          'ExpenseDeleted':        '#f44336',
          'SettlementRequested':   '#ff9800',
          'SettlementCompleted':   '#4caf50',
          'GroupInvite':           '#9c27b0',
          'MemberJoined':          '#3f51b5'
        };
        return colors[type] || '#666';
    }

    getTimeAgo(date: Date): string {
        const now = new Date();
        const diff = now.getTime() - new Date(date).getTime();
        const minutes = Math.floor(diff / 60000);
        const hours = Math.floor(minutes / 60);
        const days = Math.floor(hours / 24);
    
        if (minutes < 1) return 'Just now';
        if (minutes < 60) return `${minutes}m ago`;
        if (hours < 24) return `${hours}h ago`;
        if (days < 7) return `${days}d ago`;
        return new Date(date).toLocaleDateString();
    }

    get unreadNotifications(): Notification[] {
        return this.notifications.filter(n => !n.isRead);
    }

    get readNotifications(): Notification[] {
        return this.notifications.filter(n => n.isRead);
    }
}