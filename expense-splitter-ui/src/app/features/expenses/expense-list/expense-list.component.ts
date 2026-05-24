import { ExpenseService } from './../../../core/services/expense.service';
import { Component, OnInit } from "@angular/core";
import { Router, RouterLink } from "@angular/router";
import { AuthService } from '../../../core/services/auth.service';
import { Expense } from '../../../core/models/expense.model';
import { MatIcon } from "@angular/material/icon";
import { MatFormField, MatLabel, MatSuffix } from "@angular/material/form-field";
import { MatInput } from "@angular/material/input";
import { CommonModule } from '@angular/common';
import { MatSelectModule } from '@angular/material/select';
import { FormsModule } from '@angular/forms';
import { MatProgressSpinner } from "@angular/material/progress-spinner";
import { MatButton, MatIconButton } from "@angular/material/button";
import { MatCard, MatCardContent } from "@angular/material/card";

@Component({
    selector: 'app-expense-list',
    templateUrl: 'expense-list.component.html',
    styleUrl: 'expense-list.component.scss',
    imports: [RouterLink, MatIcon, MatInput, MatSuffix, MatFormField, CommonModule, MatSelectModule, FormsModule, MatProgressSpinner, MatButton, MatCard, MatCardContent, MatIconButton]
})
export class ExpenseListComponent implements OnInit{
    isLoading = false;
    expenses: Expense[] = [];
    filteredExpenses: Expense[] = [];
    searchQuery = '';
    selectedCategory = 0;
    
    categories = [
        { value: 0, label: 'All' },
        { value: 1, label: 'Food' },
        { value: 2, label: 'Travel' },
        { value: 3, label: 'Rent' },
        { value: 4, label: 'Utilities' },
        { value: 5, label: 'Entertainment' },
        { value: 6, label: 'Shopping' },
        { value: 7, label: 'Medical' },
        { value: 8, label: 'Other' }
    ];

    constructor(private router: Router,
        private expenseService: ExpenseService,
        private authService: AuthService
    ) {}

    ngOnInit(): void {
        this.loadExpenses();
    }

    loadExpenses() {
        const userId = this.authService.getCurrentUser()?.id;
        if(!userId)
            return;

        this.isLoading = true;
        this.expenseService.getExpensesByUser(userId).subscribe({
            next: (expenses) =>{
                this.isLoading = false;
                this.expenses = expenses;
                this.filteredExpenses = expenses;
            },
            error: () => {
                this.isLoading = false;
            }
        });
    }

    onSearch(): void {
        this.applyFilters();
    }

    onCategoryChange(): void {
        this.applyFilters();
    }

    applyFilters(): void {
        let filtered = [...this.expenses];

        if(this.searchQuery)
        {
            const query = this.searchQuery.toLowerCase();
            filtered = filtered.filter(e => e.title.toLowerCase().includes(query));
        }

        if(this.selectedCategory !== 0)
        {
            filtered = filtered.filter(e => e.category === this.selectedCategory);
        }

        this.filteredExpenses = filtered
    }

    deleteExpense(id: string, event: Event): void {
        event.stopPropagation();
        if (!confirm('Delete this expense?')) return;

        this.expenseService.deleteExpense(id).subscribe({
            next: () => {
                this.expenses = this.expenses.filter(e => e.id !== id);
                this.filteredExpenses = this.filteredExpenses.filter(e => e.id !== id);
            }
        })
    }

    editExpense(id: string, event: Event): void {
        event.stopPropagation();
        console.log("Expense ID:" + id);
        this.expenses.forEach((expense) => {console.log(expense.title + " - " + expense.id)})
        this.router.navigate(['/expenses', id, 'edit']);
    }

    getCategoryLabel(category: number): string {
        return this.categories.find(e => e.value === category)?.label || 'Other';
    }

    getCategoryIcon(category: number): string {
        const icons: Record<number, string> = {
            1: 'restaurant', 2: 'flight', 3: 'home',
            4: 'bolt', 5: 'movie', 6: 'shopping_bag',
            7: 'medical_services', 8: 'category'
        };
        return icons[category] || 'category';
    }
    
    getCategoryColor(category: number): string {
        const colors: Record<number, string> = {
            1: '#ff9800', 2: '#2196f3', 3: '#9c27b0',
            4: '#ffeb3b', 5: '#e91e63', 6: '#00bcd4',
            7: '#f44336', 8: '#607d8b'
        };
        return colors[category] || '#607d8b';
    }
    
    getTotalAmount(): number {
        return this.filteredExpenses.reduce((sum, e) => sum + e.amount, 0);
    }
}