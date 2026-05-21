import { Component, OnInit, inject } from "@angular/core";
import { Router, RouterLink } from "@angular/router";
import { Group } from "../../../core/models/group.model";
import { GroupService } from "../../../core/services/group.service";
import { AuthService } from "../../../core/services/auth.service";
import { MatButton, MatIconButton } from "@angular/material/button";
import { MatIcon } from "@angular/material/icon";
import { MatFormField, MatLabel, MatSuffix } from "@angular/material/form-field";
import { MatInput } from "@angular/material/input";
import { FormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";
import { MatProgressSpinner } from "@angular/material/progress-spinner";
import { MatCardContent, MatCard } from "@angular/material/card";
import { ThemeService } from "../../../core/services/theme.service";

@Component({
    selector: 'app-group-list',
    templateUrl: 'group-list.component.html',
    styleUrl: 'group-list.component.scss',
    imports: [MatButton, MatIconButton, RouterLink, MatIcon, MatFormField, MatLabel, MatInput, MatSuffix, FormsModule, CommonModule, MatProgressSpinner, MatCardContent, MatCard]
})
export class GroupListComponent implements OnInit {

    readonly themeService = inject(ThemeService);

    groups: Group[] = [];
    filteredGroups: Group[] = [];
    searchQuery = '';
    isLoading = false;

    constructor(
        private router: Router,
        private groupService: GroupService,
        private authService: AuthService
    ) {}

    ngOnInit(): void {
        this.loadGroups();
    }

    loadGroups() {
        const userId = this.authService.getCurrentUser()?.id;
        
        if(!userId) return;

        this.isLoading = true;
        this.groupService.getGroupsByUserId(userId).subscribe({
            next: (groups) => {
                this.groups = groups;
                this.filteredGroups = groups;
                this.isLoading = false;
            },
            error: () => {
                this.isLoading = false;
            }
        });
    }

    onSearch() {
        const query = this.searchQuery.toLowerCase();

        this.filteredGroups = this.groups.filter(g => {
            g.name.toLowerCase().includes(query) ||
            g.description?.toLowerCase().includes(query)
        });
    }
    navigateToGroups(id: string): void {
        this.router.navigate(['/groups', id])
    }

    getGroupInitial(name: string): string {
        return name.charAt(0).toUpperCase();
    }

    getAvatarColor(name: string): string {
        const colors = [
            '#3f51b5', '#e91e63', '#009688',
            '#ff5722', '#607d8b', '#9c27b0'
        ];

        const index = name.charCodeAt(0) % colors.length;
        return colors[index]; 
    }
}