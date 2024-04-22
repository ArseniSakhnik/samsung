import HTTPService from "@/services/HTTPService";

export default class ProjectsRequest extends HTTPService {
    constructor() {
        super("Projects");
    }

    getProjects(){
        return this.get("")
    }

    getProjectId(projectId){
        return this.get("" + projectId)
    }

    postProjects(body){
        return this.post("", body)
    }
}