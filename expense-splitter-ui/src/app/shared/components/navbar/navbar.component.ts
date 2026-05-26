import { Component, OnInit, inject } from "@angular/core";
import { User } from "../../../core/models/user.model";
import { AuthService } from "../../../core/services/auth.service";
import { NotificationService } from "../../../core/services/notification.service";
import { ThemeService } from "../../../core/services/theme.service";
import { Router, RouterLink, RouterLinkActive } from "@angular/router";
import { MatIcon } from "@angular/material/icon";
import { MatIconButton } from "@angular/material/button";
import { MatBadge } from "@angular/material/badge";
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { CommonModule } from "@angular/common";

@Component({
    selector: 'app-navbar',
    templateUrl: 'navbar.component.html',
    styleUrl: 'navbar.component.scss',
    imports: [RouterLink, MatIcon, RouterLinkActive, MatIconButton, MatBadge, MatMenuModule, MatDividerModule, CommonModule]
})
export class NavbarComponent implements OnInit{
    readonly themeService = inject(ThemeService);

    currentUser: User | null = null;
    unreadCount = 0;
    isMobileMenuOpen = false;

    navLinks = [
        { path: '/dashboard',    label: 'Dashboard',   icon: 'dashboard' },
        { path: '/groups',       label: 'Groups',       icon: 'group' },
        { path: '/expenses',     label: 'Expenses',     icon: 'receipt_long' },
        { path: '/settlements',  label: 'Settlements',  icon: 'payments' },
        { path: '/reports',      label: 'Reports',      icon: 'bar_chart' }
    ]

    constructor(private authService: AuthService,
        private notificationService: NotificationService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.currentUser = this.authService.getCurrentUser();
        this.loadUnreadCount();
    }

    loadUnreadCount(): void {
        this.notificationService.getUnreadCount().subscribe({
            next: (res) => {
                this.unreadCount = res.unreadCount;
            }
        });
    }

    getInitial(): string {
        return this.currentUser?.displayName.charAt(0).toUpperCase() || 'U';
    }

    toggleMobileMenu(): void {
        this.isMobileMenuOpen = !this.isMobileMenuOpen;
    }

    logout(): void {
        this.authService.logout();
    }
}