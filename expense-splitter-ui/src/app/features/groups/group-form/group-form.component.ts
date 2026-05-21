import { GroupService } from './../../../core/services/group.service';
import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';
import { MatIcon } from '@angular/material/icon';
import { MatCard, MatCardContent } from '@angular/material/card';
import { MatError, MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatButton, MatIconButton } from '@angular/material/button';
import { MatProgressSpinner } from '@angular/material/progress-spinner';

@Component({
    selector: 'app-group-form',
    templateUrl: 'group-form.component.html',
    styleUrl: 'group-form.component.scss',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        MatIcon,
        MatCard,
        MatCardContent,
        MatFormField,
        MatLabel,
        MatInput,
        MatError,
        MatButton,
        MatIconButton,
        MatProgressSpinner,
    ],
})
export class GroupFormComponent implements OnInit {
    groupFrom: FormGroup;
    isLoading = false;
    isEditMode = false;
    groupId: string | null = null;
    errorMessage = '';

    constructor(private router: Router,
        private fb: FormBuilder,
        private groupService: GroupService,
        private authService: AuthService,
        private route: ActivatedRoute
    ) {
        this.groupFrom = this.fb.group({
            name: ['', [Validators.required, Validators.minLength(2)]],
            description: [''],
        });
    }

    ngOnInit(): void {
        this.groupId = this.route.snapshot.paramMap.get('id');

        this.isEditMode = !!this.groupId;
        if(this.isEditMode && this.groupId)
        {
            this.loadGroup(this.groupId);
        }
    }

    loadGroup(groupId: string): void {
        this.isLoading = true;
        this.groupService.getGroupById(groupId).subscribe({
            next: (group) => {
                this.groupFrom.patchValue({
                    name: group.name,
                    description: group.description
                });
                this.isLoading = false;
            },
            error: (err) => {
                this.isLoading = false;
            }
        })
    }

    onSubmit(): void {
        if(this.groupFrom.invalid)
        {
            return;
        }

        this.isLoading = true;
        this.errorMessage = '';

        const currentUser = this.authService.getCurrentUser();

        if(this.isEditMode && this.groupId)
        {
            //Update
            this.groupService.updateGroup(this.groupId, this.groupFrom.value).subscribe({
                next: () =>{
                    this.isLoading = false;
                    this.router.navigate(['/groups', this.groupId]);
                },
                error: () => {
                    this.isLoading = false;
                    this.errorMessage = 'Failed to update group';
                }
            });
        }
        else{
            //Create
            this.groupService.createGroup(this.groupFrom.value).subscribe({
                next:(group) => {
                    this.isLoading = false;
                    this.router.navigate(['/groups', group.id]);
                },
                error: () => {
                    this.isLoading = false;
                    this.errorMessage = "Failed to create group.";
                }
            });
        }
    }

    goBack(): void {
        this.router.navigate(['/groups']);
    }

    get name() { return this.groupFrom.get('name'); }
}