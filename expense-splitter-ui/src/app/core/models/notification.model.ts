export interface Notification {
    id: string;
    title: string;
    message: string;
    type: string;
    isRead: boolean;
    referenceId?: string;
    referenceType?: string;
    createdAt: Date;
  }