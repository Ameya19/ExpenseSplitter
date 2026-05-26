import { Component, OnInit } from "@angular/core";
import { AuthService } from "../../core/services/auth.service";
import { ExpenseService } from "../../core/services/expense.service";
import { GroupService } from "../../core/services/group.service";
import { NotificationService } from "../../core/services/notification.service";
import { Router, RouterLink } from "@angular/router";
import { User } from "../../core/models/user.model";
import { Group } from "../../core/models/group.model";
import { Expense } from "../../core/models/expense.model";
import { Notification } from "../../core/models/notification.model";
import { MatIcon } from "@angular/material/icon";
import { MatProgressSpinner } from "@angular/material/progress-spinner";
import { CommonModule } from "@angular/common";
import { MatCard, MatCardContent, MatCardHeader, MatCardTitle, MatCardActions } from "@angular/material/card";
import { MatBadgeModule } from '@angular/material/badge';
import { MatButton, MatIconButton } from '@angular/material/button';
@Component({
    selector: 'app-dashboard',
    templateUrl: 'dashboard.component.html',
    styleUrl: 'dashboard.component.scss',
    standalone: true,
    imports: [
        RouterLink,
        MatIcon,
        MatProgressSpinner,
        CommonModule,
        MatCard,
        MatCardContent,
        MatCardHeader,
        MatCardTitle,
        MatCardActions,
        MatBadgeModule,
        MatButton,
        MatIconButton,
    ],
})
export class DashboardComponent implements OnInit {

    currentUser : User | null = null;
    groups: Group[] = [];
    recentExpenses: Expense[] = [];
    notifications: Notification[] = [];
    unreadCount: number = 0;
    isLoading = true;

    constructor(
        private authService: AuthService,
        private expenseService: ExpenseService,
        private groupService: GroupService,
        private notificationService: NotificationService,
        private router: Router
    ) {}

    ngOnInit():void {
        this.currentUser = this.authService.getCurrentUser();

        if(this.currentUser)
        {
            this.LoadDashboardData();
        }
    }

    LoadDashboardData() {
        this.isLoading = true;

        this.groupService.getGroupsByUserId(this.currentUser!.id).subscribe({
            next: (groups) => {
                this.groups = groups;
                this.isLoading = false;
            },
            error: (err) => {
                this.isLoading = false;
            }
        });

        this.expenseService.getExpensesByUser(this.currentUser!.id).subscribe({
            next: (expenses) => {
                this.recentExpenses = expenses;
                this.isLoading = false;
            },
            error: (err) => {
                this.isLoading = false;
            }
        });

        this.notificationService.getMyNotifications().subscribe({
            next: (notifications) => {
                this.notifications = notifications.slice(0, 5);
            },
            error: (err) => {}
        });

        this.notificationService.getUnreadCount().subscribe({
            next: (res) => {
                this.unreadCount = res.unreadCount;
            },
            error: (err) => {}
        });
    }

    navigateToGroup(groupId: string) {
        this.router.navigate(['/groups', groupId]);
    }

    logout() {
        this.authService.logout();
    }

    getCategoryLabel(category: number): string {
        const categories: Record<number, string> = {
            1: 'Food',
            2: 'Travel',
            3: 'Rent',
            4: 'Utilities',
            5: 'Entertainment',
            6: 'Shopping',
            7: 'Medical',
            8: 'Other'
        };

        return categories[category] || 'Other';
    }

    getCategoryIcon(category: number): string {
        const icons: Record<number, string> = {
            1: 'Food',
            2: 'Travel',
            3: 'Rent',
            4: 'Utilities',
            5: 'Entertainment',
            6: 'Shopping',
            7: 'Medical',
            8: 'Other'
        }

        return icons[category] || 'Other';
    }
}