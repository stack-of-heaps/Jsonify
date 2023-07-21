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
        </div>
    </div>
</template>

<script lang="js">
    import { defineComponent } from 'vue';

    export default defineComponent({
        data() {
            return {
                loading: false,
                classList: null,
                productList: null,
                serviceList: null,
                selectedService: null,
                selectedProduct: null
            };
        },
        created() {
            // fetch the data when the view is created and the data is
            // already being observed
            this.fetchInitialData();
        },
        watch: {
            // call again the method if the route changes
            '$route': 'fetchInitialData',

        },
        methods: {
            productSelection(selectionValue) {
                this.selectedProduct = selectionValue;
                this.fetchClasses(this.selectedService, selectionValue)
            },

            serviceSelection(selectionValue) {
                this.selectedService = selectionValue;
                this.fetchClasses(selectionValue, this.selectedProduct);
            },

            async fetchClasses(serviceFilter, productFilter) {
                if (serviceFilter === null && productFilter === null) {
                    return;
                }

                if (serviceFilter === null) {
                    serviceFilter = this.selectedService;
                }

                if (productFilter === null) {

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
                this.classList = getClassesResponse;

                console.log(getClassesResponse);
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