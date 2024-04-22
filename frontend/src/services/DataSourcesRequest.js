import HTTPService from "@/services/HTTPService";

export default class DataSourcesRequest extends HTTPService {

    constructor() {
        super("DataSources");
    }
    postContentDataFrame(body){
        return this.post("get-columns", body)
    }

    postDataFrame(query, file){
        return this.post("", `?${query}`, file)
    }

    postCustomDataFrame(body){
        return this.post("create-on-basis", body)
    }

    getDataSource(id){
        return this.get("" + id)
    }
}