import { GroupReport, UserReport } from './../../../core/models/report.model';
import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { AuthService } from "../../../core/services/auth.service";
import { ReportService } from "../../../core/services/report.service";
import { GroupService } from "../../../core/services/group.service";
import { Group } from "../../../core/models/group.model";
import { MatTabGroup, MatTab } from "@angular/material/tabs";
import { MatFormField, MatLabel, MatSuffix } from "@angular/material/form-field";
import { MatSelect, MatOption } from "@angular/material/select";
import { MatInput } from "@angular/material/input";
import { MatDatepickerInput, MatDatepickerToggle, MatDatepicker } from "@angular/material/datepicker";
import { FormsModule } from '@angular/forms';
import { MatButton } from "@angular/material/button";
import { MatIcon } from "@angular/material/icon";
import { MatProgressSpinner } from "@angular/material/progress-spinner";
import { MatCard, MatCardContent, MatCardTitle, MatCardHeader } from "@angular/material/card";
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-report-view',
    templateUrl: 'report-view.component.html',
    styleUrl: 'report-view.component.scss',
    imports: [MatTabGroup, MatTab, MatFormField, MatLabel, MatSelect, MatOption, MatSuffix, MatInput, MatDatepickerInput, MatDatepickerToggle, MatDatepicker, FormsModule, MatButton, MatIcon, MatProgressSpinner, MatCard, MatCardContent, MatCardTitle, MatCardHeader, CommonModule]
})
export class ReportViewComponent implements OnInit{
    isLoadingGroups = false;
    isLoadingUserReports = false;
    isLoadingGroupReports = false;
    fromDate: Date = new Date(new Date().getFullYear(), 0 , 1);
    toDate: Date = new Date();
    groups: Group[] = [];
    selectedGroupId: string = '';
    userReport: UserReport | null = null;
    groupReport: GroupReport | null = null;

    constructor(private router: Router,
        private authService: AuthService,
        private reportService: ReportService,
        private groupService: GroupService
    ) {}

    ngOnInit(): void {
        this.loadGroups();
        this.loadUserReports();
    }

    loadGroups(): void {
        const userId = this.authService.getCurrentUser()?.id;
        if(!userId)
            return;

        this.isLoadingGroups = true;
        this.groupService.getGroupsByUserId(userId).subscribe({
            next: (groups) => {
                this.groups = groups;
                this.isLoadingGroups = false;

                if(this.groups.length > 0)
                {
                    this.selectedGroupId = this.groups[0].id;
                    this.loadGroupReport();
                }
            },
            error: () => { this.isLoadingGroups= false; }
        });
    }

    loadGroupReport(): void {
        if(!this.selectedGroupId)
            return;

        this.isLoadingGroups = true;
        this.reportService.getGroupReport(this.selectedGroupId).subscribe({
            next: (groupReport) => {
                this.groupReport = groupReport;
                this.isLoadingGroups = false;
            },
            error: () => { this.isLoadingGroups = false; }
        });
    }

    loadUserReports(): void {
        this.isLoadingUserReports = true;
        this.reportService.getMyReport().subscribe ({
            next: (userReport) => {
                this.userReport = userReport;
                this.isLoadingUserReports = false;
            },
            error: () => { this.isLoadingUserReports = false; }
        })
    }

    loadGroupReportByDateRange(): void {
        if(!this.selectedGroupId)
            return;

        this.isLoadingGroupReports = true;
        this.reportService.getGroupReportByDateRange(this.selectedGroupId, this.fromDate.toISOString(), this.toDate.toISOString()).subscribe ({
            next: (groupReport) => {
                this.groupReport = groupReport;
                this.isLoadingGroupReports = false;
            },
            error: () => { this.isLoadingGroupReports = false; }
        })
    }

    onGroupChange(): void {
        this.loadGroupReport();
    }

    getBalanceColor(balance: number): string {
        return balance > 0 ? '#4caf50' : balance < 0 ? '#f44336' : '#666';
    }

    getCategoryIcon(category: string): string {
        const icons: Record<string, string> = {
            'Food': 'restaurant',
            'Travel': 'flight',
            'Rent': 'home',
            'Utilities': 'bolt',
            'Entertainment': 'movie',
            'Shopping': 'shopping_bag',
            'Medical': 'medical_services',
            'Other': 'category'
        };
        return icons[category] || 'category';
    }

    getCategoryColor(category: string): string {
        const colors: Record<string, string> = {
            'Food': '#ff9800',
            'Travel': '#2196f3',
            'Rent': '#9c27b0',
            'Utilities': '#ffeb3b',
            'Entertainment': '#e91e63',
            'Shopping': '#00bcd4',
            'Medical': '#f44336',
            'Other': '#607d8b'
        };
        return colors[category] || '#607d8b';
    }

    getProgressWidth(percentage: number): string {
        return `${percentage}%`;
    }
}