<template>
  <el-container>
    <el-header>
        <el-row :gutter="20" justify="center" v-if="productList">
            <el-col :span="6">
                <el-select v-model="selectedService" filterable clearable class="m-2" @change="serviceSelection" placeholder="Service" size="large">
                    <el-option v-for="(service, index) in serviceList"
                                :key="index"
                                :label="service"
                                :value="service" />
                </el-select>
            </el-col>
            <el-col :span="6">
                <el-select v-model="selectedProduct" filterable clearable class="m-2" @change="productSelection" placeholder="Product" size="large">
                    <el-option v-for="(product, index) in productList"
                                :key="index"
                                :label="product"
                                :value="product" />
                </el-select>
            </el-col>
            <el-col :span="6" v-if="classList">
                <el-select v-model="selectedClass" filterable class="m-2" placeholder="Class" size="large">
                    <el-option v-for="(classVar, index) in classList"
                                :key="index"
                                :label="classVar.fullName"
                                :value="index" />
                </el-select>
            </el-col>
        </el-row>
    </el-header>
    <el-main>
        <p>ClassData</p>
        <p>{{ jsonifiedProperty }}</p>
      <DataDisplay :classData="classProperties" />
    </el-main>
  </el-container>
</template>

<script lang="ts">
    import { defineComponent } from 'vue';
    import DataDisplay from '../src/components/DataDisplay.vue';
    import { ClassInfo } from '../src/lib/typeDefinitions'
    import { Property } from '../src/lib/typeDefinitions';
    import { ProductNames } from '../src/lib/typeDefinitions'
    import { ServiceNames } from '../src/lib/typeDefinitions'

export default defineComponent({
  name: 'App',
    components: {
      DataDisplay
  },
  data() {
            return {
                loading: false,
                classList: [] as ClassInfo[],
                productList: [] as string[],
                serviceList: [] as string[],
                selectedService: ServiceNames.None,
                selectedProduct: ProductNames.None,
                selectedClass: "",
                classProperties: {} as Property,
                jsonifiedProperty: ""
            };
        },
        created() {
            this.fetchInitialData();
        },
        watch: {
            '$route': 'fetchInitialData',
            selectedClass(newSelection){
                this.getProperties(newSelection)
            },
            classProperties(newPropertyValue){
                console.log("JSONIFIED: newPropertyValue: ", newPropertyValue)
                this.propertiesToJson(newPropertyValue)
            }
        },

        methods: {
            propertiesToJson(selectedProperty: Property): object {
                return { [selectedProperty.displayName ] : this.getSubProperties(selectedProperty)}
            },
            getSubProperties(property: Property): any {
                if (!property.properties){
                    return property.setValue;
                }

                let newObject: Record<string, any> = {};
                property.properties.forEach(element => {
                    newObject[element.displayName] = element.setValue
                })

                return newObject;
            },
            async productSelection(selectionValue: ProductNames): Promise<void> {
                this.selectedProduct = selectionValue;
                this.classList = await this.fetchClasses(this.selectedService, selectionValue)
            },

            async serviceSelection(selectionValue: ServiceNames): Promise<void> {
                this.selectedService = selectionValue;
                this.classList = await this.fetchClasses(selectionValue, this.selectedProduct);
            },

            async getProperties(classListIndex: number): Promise<Property> {
                let thisClass = this.classList[classListIndex];
                let namespace = thisClass.namespace
                let fullClassName = thisClass.fullName
                let classesBaseUrl = `https://localhost:7219/assemblyInfo/classes/${fullClassName}/properties?namespace=${namespace}`

                let response = await fetch(classesBaseUrl).then(response => response.json());

                this.classProperties = response;

                return response;
            },

            async fetchClasses(serviceFilter: ServiceNames, productFilter: ProductNames): Promise<ClassInfo[]> {
                if (!serviceFilter && !productFilter) {
                    return [];
                }

                serviceFilter = serviceFilter ?? this.selectedService;
                productFilter = productFilter ?? this.selectedProduct

                let classesBaseUrl = 'https://localhost:7219/assemblyInfo/classes?';
                let parameterAdded = false;

                if (serviceFilter != null) {
                    classesBaseUrl += `serviceName=${serviceFilter}`
                    parameterAdded = true;
                }

                classesBaseUrl += parameterAdded ? '&' : '';
                classesBaseUrl += `productName=${productFilter}`

                let getClassesResponse = await fetch(classesBaseUrl).then(response => response.json());

                return getClassesResponse.classes;
            },

            async fetchInitialData() {
                this.loading = true;

                let assemblyInfoBaseUrl = 'https://localhost:7219/assemblyInfo/';

                let allCalls = Promise.all([
                    fetch(assemblyInfoBaseUrl + 'products').then(response => response.json()),
                    fetch(assemblyInfoBaseUrl + 'services').then(response => response.json())
                ]);

                let results = await allCalls;
                this.productList = results[0];
                this.serviceList = results[1];
                this.loading = false;

                return;
            }
        }
    });
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>
