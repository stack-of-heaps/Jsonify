<script setup lang="ts">
import { onMounted,  watch, ref } from 'vue';
import DataMaster from '../src/components/DataMaster.vue';
import { ClassInfo, DotNetProperty, ProductNames, ServiceNames } from '../src/lib/typeDefinitions'

const classLoaded = ref(false);
const classList = ref<ClassInfo[]>([]);
const productList = ref<string[]>(['']);
const serviceList = ref<string[]>(['']);
const selectedService = ref<ServiceNames>(ServiceNames.None);
const selectedProduct = ref<ProductNames>(ProductNames.None)
const selectedClass = ref<number | null>(null);
const classProperties = ref<DotNetProperty>({} as DotNetProperty);
const fileInput = ref<HTMLInputElement | null>(null)
const files = ref()

onMounted(() => { fetchInitialData(); })
watch(selectedClass, async (newSelection) => { getProperties(newSelection)});

async function productSelection(selectionValue: ProductNames): Promise<void> {
    selectedProduct.value = selectionValue;
    classList.value = await fetchClasses(selectedService.value, selectionValue)
}

async function handleJsonUpload(event: Event): Promise<void> {
    let fileList = (event!.target as HTMLInputElement)!.files as FileList;
    if (!fileList || fileList.length === 0)
        return;

    let uploadedFile = fileList[0];
    if (uploadedFile.type !== 'application/json')
        return;

    let fileContents: string = await new Promise((content) => {
        let reader = new FileReader();
        reader.readAsText(uploadedFile);
        reader.onload = () => content(reader.result as string)
    })

    files.value = JSON.parse(fileContents)

    console.log("File: ", files.value)
}

async function fetchClasses(serviceFilter: ServiceNames, productFilter: ProductNames): Promise<ClassInfo[]> {
    if (!serviceFilter && !productFilter) {
        return [];
    }

    serviceFilter = serviceFilter ?? selectedService;
    productFilter = productFilter ?? selectedProduct

    let classesBaseUrl = 'https://jsonifybackend.assurity.com/assemblyInfo/classes?';
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

async function getProperties(classListIndex: number | null): Promise<DotNetProperty> {
    if (classListIndex === null){
        return {} as DotNetProperty;
    }

    classLoaded.value = false;
    let thisClass = classList.value[classListIndex];
    let namespace = thisClass.namespace
    let fullClassName = thisClass.fullName
    let classesBaseUrl = `https://jsonifybackend.assurity.com/assemblyInfo/classes/${fullClassName}/properties?namespace=${namespace}`
    let response = await fetch(classesBaseUrl).then(response => response.json());

    classProperties.value = response;
    classLoaded.value = true;

    return response;
}

async function fetchInitialData() {
    let assemblyInfoBaseUrl = 'https://jsonifybackend.assurity.com/assemblyInfo/';

    let allCalls = Promise.all([
        fetch(assemblyInfoBaseUrl + 'products').then(response => response.json()),
        fetch(assemblyInfoBaseUrl + 'services').then(response => response.json())
    ]);

    let results = await allCalls;
    productList.value = results[0];
    serviceList.value = results[1];

    return;
}
            
</script>

<style>
#app {
  font-family: Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: #2c3e50;
  margin-top: 60px;
}

body {
    background-color: #577590;
}

.overviewText {
    color: black;
    size: 8px;
}
</style>

<template>
  <el-container>
    <el-header height="300px">
        <p class="overviewText">Select a service, then (optionally) a product, then a class using the dropdown boxes below.</p>
        <p class="overviewText">Once a class is selected, it will be displayed, populated with some default values, and its corresponding JSON displayed to the right.</p>
        <p class="overviewText">All dropdown boxes can be cleared by clicking the (x) on the right-hand side when hovering over. Dropdown boxes also support filtering. To do so, simply click the dropdown box and begin typing.</p>
        <p class="overviewText">Once a class is selected, it can be manipulated as follows:</p>
        <ul class="overviewText">
            <li class="overviewText">The "trash" icon lets you set the entire field/object to null.</li>
            <li class="overviewText">The "magic wand" icon lets you restore the field/object to its default state. This can be used to bring the field back after setting it to null.</li>
            <li class="overviewText">For arrays, elements can be added and removed with the +/- buttons.</li>
        </ul>
        <el-row :gutter="20" justify="center" v-if="productList">
            <el-col :span="6">
                <p>1. Select a Service</p>
                <el-select v-model="selectedService" filterable clearable class="m-2" @change="serviceSelection" placeholder="Service">
                    <el-option v-for="(service, index) in serviceList"
                        :key="index"
                        :label="service"
                        :value="service" />
                </el-select>
            </el-col>
            <el-col :span="6">
                <p>2. Optional: Select a Product</p>
                <el-select v-model="selectedProduct" filterable clearable class="m-2" @change="productSelection" placeholder="Product">
                    <el-option v-for="(product, index) in productList"
                        :key="index"
                        :label="product"
                        :value="product" />
                </el-select>
            </el-col>
            <el-col :span="6" v-if="classList">
                <p>3. Select a class</p>
                <el-select v-model="selectedClass" filterable class="m-2" placeholder="Class">
                    <el-option v-for="(classVar, index) in classList"
                        :key="index"
                        :label="classVar.fullName"
                        :value="index" />
                </el-select>
            </el-col>
            <el-col>
                <input
                    ref="fileInput"
                    type="file"
                    v-on:change="handleJsonUpload"
                />
            </el-col>
        </el-row>
    </el-header>
    <el-main>
        <div v-if="classLoaded">
            <DataMaster :property="classProperties" :propertyProvided="classLoaded" />
        </div>
    </el-main>
  </el-container>
</template>

