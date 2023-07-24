<template>
    <div class="post">
        <div v-if="loading" class="loading">
            Loading products list...
        </div>

        <div class="post">
            <div v-if="productList">
                <el-select v-model="selectedService" filterable clearable class="m-2" @change="serviceSelection" placeholder="Service" size="large">
                    <el-option v-for="(service, index) in serviceList"
                               :key="index"
                               :label="service"
                               :value="service" />
                </el-select>
                <el-select v-model="selectedProduct" filterable clearable class="m-2" @change="productSelection" placeholder="Product" size="large">
                    <el-option v-for="(product, index) in productList"
                               :key="index"
                               :label="product"
                               :value="product" />
                </el-select>
            </div>
            <div>
                <p>
                SelectedService
                {{selectedService}}
                    </p>
            </div>
            <div v-if="classList">
                <p>Classes Present</p>
                <div>
                <el-select v-model="selectedClass" filterable clearable class="m-2" @change="fetchClass" placeholder="Class" size="large">
                    <el-option v-for="(classVar, index) in classList"
                               :key="index"
                               :label="classVar.displayName"
                               :value="index" />
                </el-select>

                </div>
            </div>
        </div>
    </div>
</template>

<script lang="ts">
    import { defineComponent } from 'vue';
    // import {ClassInfo } from '../lib/typeDefinitions';
    import { ClassInfo } from '../lib/typeDefinitions'
    import { ProductNames } from '../lib/typeDefinitions'
    import { ServiceNames } from '../lib/typeDefinitions'

    export default defineComponent({
        data() {
            return {
                loading: false,
                classList: [] as ClassInfo[],
                productList: null,
                serviceList: null,
                selectedService: ServiceNames.None,
                selectedProduct: ProductNames.None,
                selectedClass: null,
                classInfo:  null
            };
        },
        created() {
            this.fetchInitialData();
        },
        watch: {
            '$route': 'fetchInitialData',

        },
        methods: {
            async productSelection(selectionValue: ProductNames ) {
                this.selectedProduct = selectionValue;
                this.classList = await this.fetchClasses(this.selectedService, selectionValue)
            },

            async serviceSelection(selectionValue: ServiceNames) {
                this.selectedService = selectionValue;
                this.classList = await this.fetchClasses(selectionValue, this.selectedProduct);
            },

            async fetchClass(classListIndex: number){

                console.log('classListIndex: ', classListIndex);
                let thisClass = this.classList[classListIndex];
                console.log('thisClass', thisClass)
                let namespace = thisClass.namespace
                let fullClassName = thisClass.fullName
                console.log('namespace ', namespace)
                console.log('fullclassname ', fullClassName)
                
                var classesBaseUrl = `https://localhost:7219/assemblyInfo/classes/${fullClassName}/properties?namespace=${namespace}`

                let response = await fetch(classesBaseUrl).then(response => response.json());
                this.classInfo = response;
                console.log('this.classInfo ', this.classInfo)
            },

            async fetchClasses(serviceFilter: ServiceNames, productFilter: ProductNames): Promise<ClassInfo[]> {
                if (!serviceFilter && !productFilter) {
                    return [];
                }

                if (!serviceFilter) {
                    serviceFilter = this.selectedService;
                }

                if (!productFilter) {
                    productFilter = this.selectedProduct;
                }

                let classesBaseUrl = 'https://localhost:7219/assemblyInfo/classes?';
                let parameterAdded = false;

                if (serviceFilter != null) {
                    classesBaseUrl += `serviceName=${serviceFilter}`
                    parameterAdded = true;
                }

                if (productFilter != null) {
                    if (parameterAdded === true) {
                        classesBaseUrl += '&'
                    }

                    classesBaseUrl += `productName=${productFilter}`
                }

                let getClassesResponse = await fetch(classesBaseUrl).then(response => response.json());
                console.log(getClassesResponse);

                return getClassesResponse.classes;
            },

            async fetchInitialData() {
                this.productList = null;
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