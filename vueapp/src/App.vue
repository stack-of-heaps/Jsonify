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
      <DataDisplay :classData="classProperties" />
    </el-main>
  </el-container>
</template>

<script setup lang="ts">
    import { onMounted,  watch, ref } from 'vue';
    import DataDisplay from '../src/components/DataDisplay.vue';
    import { ClassInfo } from '../src/lib/typeDefinitions'
    import { Property } from '../src/lib/typeDefinitions';
    import { ProductNames } from '../src/lib/typeDefinitions'
    import { ServiceNames } from '../src/lib/typeDefinitions'

    const classLoaded = ref(false);
    const classList = ref<ClassInfo[]>([]);
    const productList = ref<string[]>(['']);
    const serviceList = ref<string[]>(['']);
    const selectedService = ref<ServiceNames>(ServiceNames.None);
    const selectedProduct = ref<ProductNames>(ProductNames.None)
    const selectedClass = ref(0);
    const classProperties = ref<Property | null>(null);

    async function productSelection(selectionValue: ProductNames): Promise<void> {
                selectedProduct.value = selectionValue;
                classList.value = await fetchClasses(selectedService.value, selectionValue)
            }

    async function fetchClasses(serviceFilter: ServiceNames, productFilter: ProductNames): Promise<ClassInfo[]> {
        if (!serviceFilter && !productFilter) {
            return [];
        }

        serviceFilter = serviceFilter ?? selectedService;
        productFilter = productFilter ?? selectedProduct

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
    }

    async function serviceSelection(selectionValue: ServiceNames): Promise<void> {
        selectedService.value = selectionValue;
        classList.value = await fetchClasses(selectionValue, selectedProduct.value);
    }

    async function getProperties(classListIndex: number): Promise<Property> {
        classLoaded.value = false;

        let thisClass = classList.value[classListIndex];
        let namespace = thisClass.namespace
        let fullClassName = thisClass.fullName
        let classesBaseUrl = `https://localhost:7219/assemblyInfo/classes/${fullClassName}/properties?namespace=${namespace}`

        let response = await fetch(classesBaseUrl).then(response => response.json());

        classProperties.value = response;
        classLoaded.value = true;

        return response;
    }
    
    async function fetchInitialData() {
        let assemblyInfoBaseUrl = 'https://localhost:7219/assemblyInfo/';

        let allCalls = Promise.all([
            fetch(assemblyInfoBaseUrl + 'products').then(response => response.json()),
            fetch(assemblyInfoBaseUrl + 'services').then(response => response.json())
        ]);

        let results = await allCalls;
        productList.value = results[0];
        serviceList.value = results[1];

        return;
    }
            
    onMounted(() => {
        fetchInitialData();
    })

    watch(selectedClass, async (newSelection) => { getProperties(newSelection)});
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
