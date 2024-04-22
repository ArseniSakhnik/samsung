import HTTPService from "@/services/HTTPService";

export default class ModelRequest extends HTTPService {
    constructor() {
        super("Models");
    }

    getModels(){
        return this.get("")
    }

    getModelsId(projectId){
        return this.get("" + projectId)
    }

    postModel(body){
        return this.post("",body)
    }
}