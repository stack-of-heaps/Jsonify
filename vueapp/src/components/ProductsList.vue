<template>
    <div class="post">
        <div v-if="loading" class="loading">
            Loading products list...
        </div>

        <div class="post">
            <div v-if="productList">
                <el-select v-model="value" class="m-2" placeholder="Service" size="large">
                    <el-option v-for="service in serviceList"
                               :key="service"
                               :label="service"
                               :value="service" />
                </el-select>
                <el-select v-model="value" class="m-2" placeholder="Product" size="large">
                    <el-option v-for="product in productList"
                               :key="product"
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
                productList: null,
                serviceList: null
            };
        },
        created() {
            // fetch the data when the view is created and the data is
            // already being observed
            this.fetchData();
        },
        watch: {
            // call again the method if the route changes
            '$route': 'fetchData'
        },
        methods: {
            async fetchData() {
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