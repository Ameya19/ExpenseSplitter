import { Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { CreateGroupRequest, Group, GroupDetail } from "../models/group.model";
import { Observable } from "rxjs";

@Injectable({ providedIn: 'root'})
export class GroupService {
    private apiUrl = `${environment.apiUrl}/groups`

    constructor(private http:HttpClient) {}

    getGroupById(groupId: string): Observable<GroupDetail> {
        return this.http.get<GroupDetail>(`${this.apiUrl}/${groupId}`);
    }

    getGroupsByUserId(userId: string): Observable<Group[]> {
        return this.http.get<Group[]>(`${this.apiUrl}/user/${userId}`);
    }

    createGroup(request: CreateGroupRequest): Observable<Group>{
        return this.http.post<Group>(this.apiUrl, request);
    }

    updateGroup(id: string, request: Partial<CreateGroupRequest>): Observable<Group>{
        return this.http.put<Group>(`${this.apiUrl}/${id}`, request);
    }

    deleteGroup(id: string): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }

    addMember(userId: string, groupId: string): Observable<void> {
        return this.http.post<void>(`${this.apiUrl}/${groupId}/members/${userId}`, {});
    }

    removeMember(userId: string, groupId: string): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${groupId}/members/${userId}`);
    }
}