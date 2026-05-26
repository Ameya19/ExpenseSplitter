import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { CreateSettlementRequest, Settlement } from "../models/settlement.model";
import { Observable } from "rxjs";

@Injectable({providedIn: 'root'})
export class SettlementService {
    private apiUrl = `${environment.apiUrl}/settlements`;
    
    constructor(private http: HttpClient) {}

    getSettlementById(userId: string): Observable<Settlement> {
        return this.http.get<Settlement>(`${this.apiUrl}/${userId}`);
    }

    createSettlement(request: CreateSettlementRequest): Observable<Settlement> {
        return this.http.post<Settlement>(this.apiUrl, request);
    }

    getSettlementsByGroup(groupId: string): Observable<Settlement[]> {
        return this.http.get<Settlement[]>(`${this.apiUrl}/group/${groupId}`);
    }

    getSettlementByUser(userId: string): Observable<Settlement[]> {
        return this.http.get<Settlement[]>(`${this.apiUrl}/user/${userId}`);
    }

    completeSettlement(id: string): Observable<Settlement> {
        return this.http.put<Settlement>(`${this.apiUrl}/${id}/complete`, {});
    }

    cancelSettlement(id: string): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}/cancel`);
    }
}