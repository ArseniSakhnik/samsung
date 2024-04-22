<template>
  <v-container fluid>
    <v-row class="mx-0 my-0" height="inherit">
      <v-list height="inherit" class="d-flex px-0 py-0">
        <template v-for="(item, index) in menuItems" :key="index">
          <v-list-item class="text" @click="handleMenuItemClick(item)" :title="item.title"/>
        </template>
      </v-list>
    </v-row>
    <v-row v-if="models[getTypeModel]">
      <template v-for="(model, n) in this.models[getTypeModel]" :key="n">
        <v-col
            class="mt-2"
            cols="12"
        >
          <v-menu
              :value="menuActive === n"
              :close-on-content-click="false"
              location="bottom"
              :activator="`#category${n}`"
          >
            <template v-slot:activator="{ on }">
              <v-btn
                  :id="`category${n}`"
                  color="indigo"
                  @click="toggleMenu(this.models[getTypeModel][n].id)"
              >
                <strong>Модель {{ this.models[getTypeModel][n].id }}</strong>
              </v-btn>
            </template>
            <v-card min-width="300">
              <v-card-title>Параметры модели</v-card-title>
              <v-list>
                <template v-for="(value, key) in this.modelData" :key="key">
                  <v-list-item v-if="key !== 'id'">
                    {{ key }}: {{ value }}
                  </v-list-item>
                </template>
              </v-list>
            </v-card>
          </v-menu>
        </v-col>
        <template v-if="models[getTypeModel][n].graphics">
          <v-col
              v-for="(j, index) in models[getTypeModel][n].graphics.length"
              :key="`${n}${index}`"
              cols="4"
              md="2"
          >
            <v-sheet height="180">
              <v-img
                  :src="`https://localhost:7268${models[getTypeModel][n].graphics[j-1]}`"
                  @click="openImageDialog(`https://localhost:7268${models[getTypeModel][n].graphics[j-1]}`)"/>
            </v-sheet>
          </v-col>
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
          <v-btn @click="closeModal()">
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
                    :item-props="itemProps"></v-select>
          <v-select label="Выберите колонку времени" v-model="timeColumn" :items="availableColumns"/>
          <v-row>
            <v-col v-for="(value, key) in checkboxes" :key="key" cols="6">
              <v-checkbox v-model="checkboxes[key]" :label="key"/>
            </v-col>
          </v-row>
          <v-select
              v-model="selectedColumns"
              :items="availableColumns"
              label="Выберите колонки датасета"
              multiple
              chips
          ></v-select>
        </v-card-text>
        <v-card-actions>
          <v-btn @click="closeModal()">
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
          <v-btn @click="addNewModel(selectedModel)" :disabled="buttonDisabled">
            Добавить
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <v-dialog v-model="imageDialog" max-width="1000" max-height="800">
      <v-card>
        <v-card-actions>
          <v-btn @click="compareResults(imageUrl, getTypeModel)">Сравнить результаты</v-btn>
        </v-card-actions>
        <v-img :src="imageUrl" contain/>
        <v-col v-if = "compareClicked"
               v-for="(graphic, index) in matchingGraphics"
               :key="index"
        >
          <v-sheet>
            <v-img :src="`https://localhost:7268${graphic}`" contain/>
          </v-sheet>
        </v-col>
      </v-card>
    </v-dialog>
  </v-container>
</template>
<script>
import DataSourcesRequest from "@/services/DataSourcesRequest";
import ProjectsRequest from "@/services/ProjectsRequest";
import axios from "axios";
import ModelRequest from "@/services/ModelRequest";

export default {
  name: 'ProjectView',

  data: () => ({
    models: [],
    projectDatasets: [],
    selectedModel: null,
    availableModels: ['knn', 'isolation-forest', 'lof', 'auto-encoder', 'gan', 'siamese'],
    selectedDataset: {path: ''},
    selectedAlgorithm: null,
    selectedColumn: null,
    text: null,
    newModel: false,
    addDataset: false,
    customDataset: false,
    menuActive: -1,
    menuItems: [{title: "Добавить датасет"}, {title: "Настроить датасет"}, {title: "Обучить модель"}],
    graphics: [],
    availableColumns: [],
    selectedColumns: [],
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
    compareClicked: false,
    matchingGraphics: []
  }),

  computed: {
    getProjectId() {
      const {selectedProjectID} = this.$route.params;
      return selectedProjectID
    },

    getTypeModel() {
      const {type} = this.$route.params;
      return type
    }
  },

  created() {
    this.initialize()
  },

  methods: {
    compareResults(url, getTypeModel) {
      const matchingModels = this.models[getTypeModel];
      console.log(matchingModels)
      if (matchingModels && Array.isArray(matchingModels)) {
        const matchingGraphics = [];
        for (const model of matchingModels) {
          const graphics = model.graphics;
          for (const graphic of graphics) {
            const urlFileName = url.substring(url.lastIndexOf("/") + 1);
            const graphicFileName = graphic.substring(graphic.lastIndexOf("/") + 1);
            if (urlFileName === graphicFileName) {
              matchingGraphics.push(graphic);
            }
          }
        }
        console.log(matchingGraphics)
        this.matchingGraphics = matchingGraphics
        this.compareClicked = true
        return console.log(this.compareClicked);
      } else {
        return [];
      }
    },


    closeModal() {
      this.addDataset = false;
      this.newModel = false;
      this.customDataset = false
      this.selectedModel = null
      this.availableModels = []
      this.selectedDataset = {path: ''}
      this.availableColumns = []
      this.selectedColumns = []
      this.selectedAlgorithm = null
      this.selectedColumn = null
      this.text = null
      this.timeColumn = ''
    },

    openImageDialog(imageUrl) {
      this.compareClicked = false
      this.matchingGraphics = null
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
        "dataSourceId": 6,
        "projectId": 10,
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
      this.newModel = false
      await this.initialize()
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

    async toggleMenu(id) {
      this.menuActive = (this.menuActive === id) ? -1 : id;
      this.modelData = {
        "Гиперпараметр nNeighbors": 12,
        "Алгоритм КНН": "auto",
        "Перцентиль": 95,
        "Колонки датасета": 'ch3rate, cursens2, fl, nrst, ptrend2, lastevent_ch2_2',
        "Показатели космической погоды": 'Dst'
      }
      return this.modelData
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
      cal = await this.getDatasetProjectForID();
      this.projectDatasets = cal

    },
  }

}
</script>

<style>
</style>