import { Component, OnInit } from "@angular/core";
import { Settlement } from "../../../core/models/settlement.model";
import { AuthService } from "../../../core/services/auth.service";
import { SettlementService } from "../../../core/services/settlement.service";
import { Group } from "../../../core/models/group.model";
import { BalanceService } from "../../../core/services/balance.service";
import { SettlementSuggestion } from "../../../core/models/balance.model";
import { GroupService } from "../../../core/services/group.service";
import { MatCard, MatCardContent } from "@angular/material/card";
import { MatFormField, MatLabel, MatSuffix } from "@angular/material/form-field";
import { FormsModule } from "@angular/forms";
import { MatOption } from "@angular/material/select";
import { MatIcon } from "@angular/material/icon";
import { MatProgressSpinner } from "@angular/material/progress-spinner";
import { CommonModule } from '@angular/common';
import { MatButton } from "@angular/material/button";

@Component({
    selector: 'app-settlement-list',
    templateUrl: 'settlement-list.component.html',
    styleUrl: 'settlement-list.component.scss',
    imports: [MatCard, MatCardContent, MatFormField, MatLabel, FormsModule, MatOption, MatIcon, MatSuffix, MatProgressSpinner, MatButton, CommonModule]
})
export class SettlementListComponent implements OnInit {
    isLoading = false;
    settlements: Settlement[] = [];
    suggestions: SettlementSuggestion[] = [];
    groups: Group[] = [];
    isLoadingGroups = false;
    isLoadingSuggestions = false;
    currenUserId = ''
    selectedGroupId = '';


    constructor(
        private authService: AuthService,
        private settlementService: SettlementService,
        private balanceService: BalanceService,
        private groupService: GroupService
    ) {}

    ngOnInit(): void {
        this.currenUserId = this.authService.getCurrentUser()?.id || '';
        this.loadGroups();
        this.loadMySettlements();
    }

    loadGroups(): void {
        this.isLoadingGroups = true;
        this.groupService.getGroupsByUserId(this.currenUserId).subscribe({
            next: (groups) => {
                this.groups = groups;
                this.isLoadingGroups = false;

                if(this.groups.length > 0){
                    this.selectedGroupId = this.groups[0].id;
                    this.onGroupChange();
                }
            },
            error: () => {
                this.isLoadingGroups = false;
            }
        })
    }

    loadMySettlements(): void {
        this.isLoading = true;
        this.settlementService.getSettlementByUser(this.currenUserId).subscribe({
            next: (settlements) => {
                this.settlements = settlements;
                this.isLoading = false;
            },
            error: () => { this.isLoading = false; }
        })
    }

    onGroupChange(): void {
        if( !this.selectedGroupId)
            return;
        this.loadSuggestions();
    }

    loadSuggestions(): void {
        this.isLoadingSuggestions = true;
        this.balanceService.getSettlementSuggestions(this.selectedGroupId).subscribe({
            next: (suggestions) => {
                this.suggestions = suggestions;
                this.isLoadingSuggestions = false;
            },
            error: () => {
                this.isLoadingSuggestions = false;
            }
        });
    }

    settleUp(suggestion: SettlementSuggestion): void {
        if (!confirm(`Confirm payment of ₹${suggestion.amount} to ${suggestion.toUserName}?`)) 
            return;

        this.settlementService.createSettlement({
            groupId: this.selectedGroupId,
            fromUserId: suggestion.fromUserId,
            toUserId: suggestion.toUserId,
            amount: suggestion.amount,
            note: 'Settled via app'
        }).subscribe({
            next: (settlement) => {
                this.settlementService.completeSettlement(settlement.id).subscribe({
                    next: () => {
                        this.loadMySettlements();
                        this.loadSuggestions();
                    }
                });
            }
        });
    }

    completeSettlement(id: string): void {
        this.settlementService.completeSettlement(id).subscribe({
            next: () => {
                this.loadMySettlements();
                this.loadSuggestions();
            }
        });
    }

    cancelSettlement(id: string): void {
        this.settlementService.cancelSettlement(id).subscribe({
            next: () => {
                this.settlements = this.settlements.filter(x => x.id !== id);
            }
        })
    }

    getStatusColor(status: string): string {
        const colors: Record<string, string> = {
          'Pending': '#ff9800',
          'Completed': '#4caf50',
          'Cancelled': '#f44336'
        };
        return colors[status] || '#666';
    }
    
    getStatusIcon(status: string): string {
        const icons: Record<string, string> = {
          'Pending': 'hourglass_empty',
          'Completed': 'check_circle',
          'Cancelled': 'cancel'
        };
        return icons[status] || 'info';
    }
    
    isMyDept(settlement: Settlement): boolean {
        return settlement.fromUserName === this.authService.getCurrentUser()?.displayName;
    }
}