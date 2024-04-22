<template>
  <v-container fluid>
    <v-row class="mx-0 my-0" height="inherit">
      <v-list height="inherit" class="d-flex px-0 py-0">
        <template v-for="(item, index) in menuItems" :key="index">
          <v-list-item class="text" @click="handleMenuItemClick(item)" :title="item.title"/>
        </template>
      </v-list>
    </v-row>
    <v-row v-if="models !== undefined">
      <template v-for="(model, n) in Object.keys(models)" :key="n">
        <v-col class="mt-2" cols="12">
          <v-menu
              :value="menuActive === n"
              :close-on-content-click="false"
              location="bottom"
              :activator="`#category${n}`">
            <template v-slot:activator="{ on }">
              <v-btn
                  :id="`category${n}`"
                  color="indigo"
                  @click="toggleMenu(model)">
                <strong>{{ getModelName(n) }}</strong>
              </v-btn>
              <v-btn
                  :id="`category${n}`"
                  color="second"
                  @click="openModelsView(getModelName(n))"
                  class="ml-3">
                <strong>Посмотреть остальное</strong>
              </v-btn>
            </template>
            <v-card min-width="300">
              <v-card-title>Параметры последней модели</v-card-title>
              <v-list>
                <template v-for="(value, key) in modelData" :key="key">
                  <v-list-item v-if="key !== 'id'">
                    {{ key }}: {{ value }}
                  </v-list-item>
                </template>
              </v-list>
            </v-card>
          </v-menu>
        </v-col>
        <template v-if="models[model]">
          <v-row class="justify-start px-3">
            <v-col
                v-for="(path) in models[model][0].graphics"
                :key="`${model}${path}`"
                cols="12"
                sm="6"
                md="3">
              <v-sheet height="280" width="400">
                <v-img
                    :src="`https://localhost:7268${path}`"
                    @click="openImageDialog(`https://localhost:7268${path}`)"/>
              </v-sheet>
            </v-col>
          </v-row>
        </template>
      </template>
    </v-row>
    <v-dialog v-model="addDataset" persistent width="auto">
      <v-card width="600px">
        <v-card-title>Добавить датасет</v-card-title>
        <v-card-text>
          <v-text-field label="Название датасета" v-model="text"/>
          <v-text-field label="Колонка времени" v-model="timeColumn"/>
          <v-file-input v-model="addedDataset"/>
        </v-card-text>
        <v-card-actions>
          <v-btn @click="addDataset=false">
            Отмена
          </v-btn>
          <v-btn @click="addNewDataset()">
            Добавить
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <v-dialog v-model="customDataset" persistent width="auto">
      <v-card width="600px">
        <v-card-title>Настроить датасет</v-card-title>
        <v-card-text>
          <v-text-field label="Название нового датасета" v-model="text"/>
          <v-select v-model="selectedDataset"
                    label="Выберите датасет"
                    :items="projectDatasets"
                    @update:modelValue="acceptBaseDataset(selectedDataset)"
                    :item-props="itemProps"/>
          <v-select label="Выберите колонку времени" v-model="timeColumn" :items="availableColumns"/>
          <v-row>
            <v-col v-for="(value, key) in checkboxes" :key="key" cols="3">
              <v-checkbox v-model="checkboxes[key]" :label="key"/>
            </v-col>
          </v-row>
          <v-select
              v-model="selectedColumns"
              :items="availableColumns"
              label="Выберите колонки датасета"
              multiple
              chips/>
        </v-card-text>
        <v-card-actions>
          <v-btn @click="customDataset=false">
            Отмена
          </v-btn>
          <v-btn @click="addNewCustomDataset()">
            Добавить
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <v-dialog v-model="newModel" persistent width="auto">
      <v-card width="600px">
        <v-card-title>Укажите данные и гиперпараметры модели</v-card-title>
        <v-card-text>
          <v-select v-model="selectedModel"
                    label="Выберите модель"
                    :items="availableModels"/>
          <v-select v-model="selectedDataset"
                    label="Выберите датасет"
                    :items="projectDatasets"
                    @update:modelValue="acceptBaseDataset(selectedDataset)"
                    :item-props="itemProps"/>
          <v-select label="Выберите колонку времени" v-model="timeColumn" :items="availableColumns"/>
          <v-select
              v-model="selectedColumns"
              :items="availableColumns"
              label="Выберите колонки датасета"
              multiple
              chips/>
        </v-card-text>
        <div class="d-flex justify-center">
          <v-progress-circular indeterminate v-if="buttonDisabled"/>
        </div>
        <v-card-actions>
          <v-btn @click="newModel=false" :disabled="buttonDisabled">
            Отмена
          </v-btn>
          <v-btn @click="addNewModel(availableModels.indexOf(selectedModel))" :disabled="buttonDisabled">
            Добавить
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <v-dialog v-model="imageDialog" max-width="1000" max-height="800">
      <v-card>
        <v-card-actions>
          <v-btn>Сравнить результаты</v-btn>
        </v-card-actions>
        <v-img :src="imageUrl" contain/>
      </v-card>
    </v-dialog>
  </v-container>
</template>
<script>
import ProjectsRequest from "@/services/ProjectsRequest";
import axios from "axios";
import DataSourcesRequest from "@/services/DataSourcesRequest";
import {mapActions} from "vuex";
import ModelRequest from "@/services/ModelRequest";

export default {
  name: 'ProjectView',

  data: () => ({
    models: [],
    projectDatasets: [],
    selectedModel: null,
    availableModels: [
      'k-nearest neighbors',
      'local outlier factor',
      'isolation forest',
      'Gaussian mixture model',
      'kernel density estimation',
      'one-class support vector machine',
      'DBSCAN',
      'OPTICS',
      'Gaussian anomaly detection',
      'k-means anomaly detection',
      'hierarchical anomaly detection',
      'subspace anomaly detection',
      'autoencoder anomaly detection',
      'PCA anomaly detection',
      'SVD anomaly detection',
      'convex hull anomaly detection',
      'minimum volume anomaly detection',
      'half-space anomaly detection',
      'feature bagging',
      'isolation forest ensemble',
        'anomaly detection with multiple classifiers',
      'Тестовый автэнкодер',
      'Feedforward Neural Networks',
      'Generative Adversarial Networks',
      'Recurrent Neural Networks',
      'Convolutional Neural Networks'
    ],
    selectedDataset: null,
    selectedAlgorithm: null,
    text: null,
    drawer: null,
    createProject: false,
    newModel: false,
    addDataset: false,
    customDataset: false,
    menuActive: -1,
    menuItems: [{title: "Добавить датасет"}, {title: "Настроить датасет"}, {title: "Обучить модель"}],
    availableColumns: [],
    selectedColumns: null,
    addedDataset: null,
    timeColumn: '',
    checkboxes: {
      isDstLoaded: false,
      isNormalize: false,
      isLoadDst: false,
      isLoadKp: false,
      isLoadAp: false,
      isLoadWolf: false,
      isNaDropped: false
    },
    modelData: {},
    buttonDisabled: false,
    imageDialog: false,
    imageUrl: '',

  }),

  created() {
    this.initialize()
  },
  computed: {
    getProjectId() {
      const {selectedProjectID} = this.$route.params;
      return selectedProjectID;
    }
  },

  methods: {
    ...mapActions(['updateAppBarTitle']),
    changeAppBarTitle(newTitle) {
      this.updateAppBarTitle(newTitle);
    },

    openModelsView(typeModel) {
      let selectedProjectID = parseInt(this.getProjectId);
      let type = typeModel
      this.$router.push({name: 'modelView', params: {selectedProjectID, type}}).catch(() => {
      });
    },

    openImageDialog(imageUrl) {
      this.imageUrl = imageUrl;
      this.imageDialog = true;
    },

    itemProps(item) {
      const pathComponents = item.path.split('\\');
      const fileName = pathComponents[pathComponents.length - 1];
      return {
        title: fileName,
      };
    },

    getModelName(index) {
      const modelKeys = Object.keys(this.models);
      if (modelKeys.length > index && index >= 0) {
        return modelKeys[index];
      } else {
        return ''
      }
    },

    async acceptBaseDataset(item) {
      if (item.id) {
        this.availableColumns = await this.getDatasetForID(item.id)
      }
    },

    handleMenuItemClick(item) {
      if (item.title === 'Добавить датасет') {
        this.addDataset = true
      } else if (item.title === "Обучить модель") {
        this.newModel = true
      } else {
        this.customDataset = true
      }
    },

    async getDatasetForID(id) {
      const dataset = new DataSourcesRequest()
      let datasetData
      await dataset.getDataSource(id).then(response => {
        datasetData = response.data.columns.map(column => column);
      }).catch(x => console.log(x))
      return datasetData
    },

    async getDatasetProjectForID() {
      const project = new ProjectsRequest()
      let datasets
      let titleProject
      await project.getProjectId(this.getProjectId).then(response => {
        datasets = response.data.dataSources.map(dataset => dataset)
        titleProject = response.data.title
      }).catch(x => console.log(x))
      this.changeAppBarTitle(titleProject)
      return datasets
    },

    async addNewDataset() {
      const formData = new FormData()
      formData.append("File", this.addedDataset)
      await axios.post(`${process.env["BACKEND_URL"]}/api/DataSources?projectId=${this.getProjectId}&timeColumn=${this.timeColumn}&dataSourceTitle=${this.text}`, formData).finally(() => {
        this.addDataset = false
        this.text = ''
      })
    },

    async addNewCustomDataset() {
      const dataSourcesRequest = new DataSourcesRequest()
      let body = {
        "dataSourceId": this.selectedDataset.id,
        "projectId": this.getProjectId,
        "title": this.text,
        "isDstLoaded": this.checkboxes.isDstLoaded,
        "isNormalize": this.checkboxes.isNormalize,
        "isLoadDst": this.checkboxes.isLoadDst,
        "isLoadKp": this.checkboxes.isLoadKp,
        "isLoadAp": this.checkboxes.isLoadAp,
        "isLoadWolf": this.checkboxes.isLoadWolf,
        "isNaDropped": this.checkboxes.isNaDropped,
        "timeColumn": this.timeColumn,
        "columns": this.selectedColumns
      }
      await dataSourcesRequest.postCustomDataFrame(body).catch(x => {
        console.log(x)
      }).finally(() => {
        this.text = ''
        this.customDataset = false
      })
    },

    async addNewModel(modelType) {
      let modelRequest = null;
      let body = {}
      this.buttonDisabled = true
      modelRequest = new ModelRequest();
      body = {
        "projectId": parseInt(this.getProjectId),
        "modelType": modelType + 1,
        "dataSourceId": this.selectedDataset.id,
        "columns": this.selectedColumns,
        "spaceWeatherColumns": ["dst", "kp", "ap", "wolf"]
      }
      await modelRequest.postModel(body).catch(x => console.log(x)).finally(async () => {
        this.text = ''
        this.newModel = false
        this.buttonDisabled = false
        await this.initialize()
      })
    },

    async toggleMenu(id) {
      this.menuActive = (this.menuActive === id) ? -1 : id;
      this.modelData = await this.getModel(id)
      return this.modelData
    },

    async getModel(id) {
      const model = new ModelRequest()
      let data
      await model.getModelsId(id).then(response => {
        data = response.data
      }).catch(x => console.log(x)).finally(() => {
      })
      return data
    },

    async getModelsForID() {
      const project = new ModelRequest()
      let models
      await project.getModelsId(this.getProjectId).then(response => {
        models = response.data;
      }).catch(x => console.log(x))
      return models
    },

    async initialize() {
      let cal;
      cal = await this.getModelsForID();
      this.models = cal;
      for (let key in this.models) {
        if (!this.models[key] || !this.models[key].length) {
          this.models[key] = [{}];
        } else {
          let maxIdModel = this.models[key].reduce((max, model) => (model.id > max.id ? model : max), this.models[key][0]);
          this.models[key] = [maxIdModel];
        }
      }
      console.log(this.models)
      cal = await this.getDatasetProjectForID();
      this.projectDatasets = cal

    }
    ,
  }
}
</script>
<style scoped>

</style>