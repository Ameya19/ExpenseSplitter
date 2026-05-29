import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { Observable } from "rxjs";
import { GroupReport, UserReport } from "../models/report.model";

@Injectable({providedIn: 'root'})
export class ReportService{
    private apiUrl = `${environment.apiUrl}/reports`;

    constructor(private http: HttpClient){}

    getGroupReportByDateRange(groupId: string, from: string, to: string): Observable<GroupReport> {
        return this.http.get<GroupReport>(`${this.apiUrl}/group/${groupId}/range?from=${from}&to=${to}`);
    }

    getGroupReport(groupId: string): Observable<GroupReport> {
        return this.http.get<GroupReport>(`${this.apiUrl}/group/${groupId}`);
    }

    getMyReport(): Observable<UserReport> {
        return this.http.get<UserReport>(`${this.apiUrl}/me`);
    }
}