import { GroupService } from './../../../core/services/group.service';
import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { Router } from "@angular/router";
import { ExpenseService } from "../../../core/services/expense.service";
import { AuthService } from "../../../core/services/auth.service";
import { MatButton, MatIconButton } from '@angular/material/button';
import { MatIconModule } from "@angular/material/icon";
import { MatCard, MatCardContent } from "@angular/material/card";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatFormField, MatLabel, MatError, MatSuffix, MatPrefix } from "@angular/material/form-field";
import { MatInput } from "@angular/material/input";
import { MatSelect, MatOption } from "@angular/material/select";
import { MatDatepickerModule } from '@angular/material/datepicker';
import { CommonModule } from "@angular/common";
import { MatProgressSpinner } from "@angular/material/progress-spinner";
import { Group } from "../../../core/models/group.model";

@Component({
    selector: 'app-expense-form',
    templateUrl: 'expense-form.component.html',
    styleUrl: 'expense-form.component.scss',
    standalone: true,
    imports: [MatButton, MatIconButton, MatIconModule, MatCard, MatCardContent, FormsModule, ReactiveFormsModule, MatFormField, MatLabel, MatInput, MatError, MatSelect, MatOption, MatSuffix, MatDatepickerModule, CommonModule, MatProgressSpinner, MatPrefix]
})
export class ExpenseFormComponent implements OnInit {
    expenseForm: FormGroup;
    groupId: string | null = null;
    expenseId: string | null = null;
    isEditMode = false;
    isLoading = false;
    errorMessage = '';
    groups: Group[] = [];
    isGroupsLoading = false;

    categories = [
        { value: 1, label: 'Food' },
        { value: 2, label: 'Travel' },
        { value: 3, label: 'Rent' },
        { value: 4, label: 'Utilities' },
        { value: 5, label: 'Entertainment' },
        { value: 6, label: 'Shopping' },
        { value: 7, label: 'Medical' },
        { value: 8, label: 'Other' }
    ];

    splitType = [
        { value: 1, label: 'Equal' },
        { value: 2, label: 'Percentage' },
        { value: 3, label: 'Exact' }
    ];

    constructor(private fb: FormBuilder,
        private route: ActivatedRoute,
        private router: Router,
        private expenseService: ExpenseService,
        private authService: AuthService,
        private groupService: GroupService
    ) {
        this.expenseForm = this.fb.group({
            title: ['', [Validators.required, Validators.minLength(2)]],
            amount: ['', [Validators.required, Validators.min(1)]],
            category: [1, Validators.required],
            splitType: [1, Validators.required],
            date: [new Date(), Validators.required],
            groupId: ['', Validators.required]
        })
    }

    ngOnInit(): void {
        this.groupId = this.route.snapshot.queryParamMap.get('groupId');
        if (this.groupId) {
            this.expenseForm.patchValue({ groupId: this.groupId });
        }

        this.loadGroups();

        this.expenseId = this.route.snapshot.paramMap.get('id');
        this.isEditMode = !!this.expenseId;

        if(this.isEditMode && this.expenseId)
        {
            this.loadExpense(this.expenseId);
        }
    }

    loadExpense(expenseId: string): void {
        this.isLoading = true;
        this.expenseService.getExpenseById(expenseId).subscribe ({
            next: (expense) => {
                this.expenseForm.patchValue({
                    title: expense.title,
                    amount: expense.amount,
                    category: expense.category,
                    splitType: expense.splitType,
                    date: expense.date,
                    groupId: expense.groupId
                });
                this.isLoading = false;
            },
            error: () => {
                this.isLoading = false;
            }
        })
    }

    loadGroups(): void {
        const userId = this.authService.getCurrentUser()?.id;
        if(!userId)
            return

        this.isGroupsLoading = true;
        this.groupService.getGroupsByUserId(userId).subscribe({
            next: (groups) => {
                this.groups= groups;
                this.isGroupsLoading = false;
            },
            error: () => {
                this.isGroupsLoading = false;
            }
        });
    }

    onSubmit(): void {
        if(this.expenseForm.invalid)
        {
            return;
        }

        this.isLoading = true;
        this.errorMessage = '';
        
        const currentUser = this.authService.getCurrentUser();
        const formValue = this.expenseForm.value;

        if(this.isEditMode && this.expenseId)
        {
            this.expenseService.updateExpense(this.expenseId, formValue).subscribe({
                next: () => {
                    this.isLoading = false;
                    this.router.navigate(['/expenses']);
                },
                error: () => {
                    this.isLoading = false;
                    this.errorMessage = "Failed to update expense.";
                }
            })
        }
        else{
            this.expenseService.createExpense({
            ...formValue, paidByUserId: currentUser!.id}).subscribe({
                next: () => {
                    this.isLoading = false;
                    if (this.groupId) {
                        this.router.navigate(['/groups', this.groupId]);
                    } else {
                        this.router.navigate(['/expenses']);
                    }
                },
                error: () => {
                    this.isLoading = false;
                    this.errorMessage = 'Failed to create expense.';
                }
            });
        }
    }

    goBack(): void {
        if(this.groupId){
            this.router.navigate(['/groups',this.groupId]);
        }
        else{
            this.router.navigate(['/expenses']);
        }
    }

    get title() { return this.expenseForm.get('title'); }
    get amount() { return this.expenseForm.get('amount'); }
    get groupIdControl() { return this.expenseForm.get('groupId'); }
}