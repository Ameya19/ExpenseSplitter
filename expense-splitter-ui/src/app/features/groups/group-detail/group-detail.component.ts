import { BalanceService } from './../../../core/services/balance.service';
import { GroupService } from './../../../core/services/group.service';
import { ActivatedRoute, Router, RouterLink, ɵEmptyOutletComponent } from '@angular/router';
import { Component, OnInit } from "@angular/core";
import { GroupDetail } from "../../../core/models/group.model";
import { Expense } from "../../../core/models/expense.model";
import { Balance, SettlementSuggestion } from "../../../core/models/balance.model";
import { ExpenseService } from '../../../core/services/expense.service';
import { AuthService } from '../../../core/services/auth.service';
import { MatProgressSpinner } from "@angular/material/progress-spinner";
import { CommonModule } from '@angular/common';
import { MatIcon } from "@angular/material/icon";
import { MatButton } from "@angular/material/button";
import { MatCard } from "@angular/material/card";
import { MatTabsModule } from '@angular/material/tabs';

@Component({
    selector: 'app-group-detail',
    templateUrl: 'group-detail.component.html',
    styleUrl: 'group-detail.component.scss',
    imports: [MatProgressSpinner, CommonModule, MatIcon, RouterLink, MatButton, MatCard, MatTabsModule]
})
export class GroupDetailComponent implements OnInit{
    group: GroupDetail | null = null;
    expenses: Expense[] = [];
    balances: Balance[] = [];
    suggestions: SettlementSuggestion[] = [];
    isLoading = false;
    groupId = ''

    constructor(private router:Router,
        private route: ActivatedRoute,
        private groupService: GroupService,
        private expenseService: ExpenseService,
        private balanceService: BalanceService,
        private authService: AuthService
    ) {}

    ngOnInit(): void {
        this.groupId = this.route.snapshot.paramMap.get('id')!;
        this.loadGroupData();
    }

    loadGroupData(): void {
        this.isLoading = true;
        
        this.groupService.getGroupById(this.groupId).subscribe({
            next: (group) => {
                this.group = group;
                this.isLoading = false;
            },
            error: () => {
                this.isLoading = false;
            }
        });

        this.expenseService.getExpensesByGroup(this.groupId).subscribe({
            next: (expenses) => {
                this.expenses = expenses;
            },
            error: () => {}
        });

        this.balanceService.getGroupBalances(this.groupId).subscribe({
            next: (balances) => {
                this.balances = balances;
            },
            error: () => {}
        });

        this.balanceService.getSettlementSuggestions(this.groupId).subscribe({
            next: (suggestions) => {
                this.suggestions = suggestions;
            },
            error: () => {}
        });
    }

    deleteGroup() {
        if (!confirm('Are you sure you want to delete this group?')) 
            return;

        this.groupService.deleteGroup(this.groupId).subscribe({
            next: () => {
                alert('Group has been deleted successfully.');
                this.router.navigate(['/groups'], { replaceUrl: true });
            },
            error: () => {
                alert('Failed to delete group. Please try again.');
            }
        });
    }

    getCategoryIcon(category: number) {
        const icons: Record<number, string> = {
            1: 'restaurant', 2: 'flight', 3: 'home',
            4: 'bolt', 5: 'movie', 6: 'shopping_bag',
            7: 'medical_services', 8: 'category'
        };

        return icons[category] || 'category';
    }

    getCategoryLabel(category: number) {
        const categories: Record<number, string> = {
            1: 'Food', 2: 'Travel', 3: 'Rent',
            4: 'Utilities', 5: 'Entertainment',
            6: 'Shopping', 7: 'Medical', 8: 'Other'
        };
        
        return categories[category] || 'Other';
    }

    getBalanceColor(balance: number): string {
        return balance > 0 ? '#4caf50' : balance < 0 ? '#f44336' : '666'
    }

    getCurrentUserId(): string {
        return this.authService.getCurrentUser()?.id || '';
    }
}