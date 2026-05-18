import { Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { Balance, SettlementSuggestion } from "../models/balance.model";

@Injectable({providedIn: 'root'})
export class BalanceService{
    private apiUrl = `${environment.apiUrl}/balances`;

    constructor(private http: HttpClient) {}

    getGroupBalances(groupId: string): Observable<Balance[]> {
        return this.http.get<Balance[]>(`${this.apiUrl}/group/${groupId}`);
    }

    getSettlementSuggestions(groupId: string): Observable<SettlementSuggestion[]> {
        return this.http.get<SettlementSuggestion[]>(`${this.apiUrl}/group/${groupId}/suggestions`);
    }
}