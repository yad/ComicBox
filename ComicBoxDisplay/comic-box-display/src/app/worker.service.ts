import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';

@Injectable()
export class WorkerService {

    private progress: any;

    constructor(private http: Http) { }

    public getThumbnailWorkerStatus() {
        return this.http.get("api/progress/thumbnailworkerstatus").map(response => {
            this.progress = response.json();
            this.progress.ratio = this.progress.inProgressCompletedCount === this.progress.inProgressTotalCount ? 100 : Math.ceil(this.progress.inProgressCompletedCount / this.progress.inProgressTotalCount * 100);
            return this.progress;
        });
    }

    public isInProgress(): boolean {
        if (!this.progress) {
            return true; //unk
        }

        return this.progress.isInProgress;
    }

}
