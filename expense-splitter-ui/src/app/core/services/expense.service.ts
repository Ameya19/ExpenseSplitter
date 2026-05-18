import { Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { createExpenseRequest, Expense } from "../models/expense.model";
import { Observable } from "rxjs";

@Injectable({providedIn: 'root'})
export class ExpenseService {
    private apiUrl = `${environment.apiUrl}/expenses`;

    constructor(private http: HttpClient) {}

    createExpense(request: createExpenseRequest): Observable<Expense> {
        return this.http.post<Expense>(this.apiUrl, request);
    }

    getExpenseById(expenseId: string): Observable<Expense> {
        return this.http.get<Expense>(`${this.apiUrl}/${expenseId}`);
    }

    getExpensesByGroup(groupId: string): Observable<Expense[]> {
        return this.http.get<Expense[]>(`${this.apiUrl}/${groupId}`);
    }

    getExpensesByUser(userId: string): Observable<Expense[]> {
        return this.http.get<Expense[]>(`${this.apiUrl}/${userId}`);
    }

    updateExpense(expenseId: string, request: Partial<createExpenseRequest>): Observable<Expense> {
        return this.http.put<Expense>(`${this.apiUrl}/${expenseId}`, request);
    }

    deleteExpense(expenseId: string): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${expenseId}`);
    }
}