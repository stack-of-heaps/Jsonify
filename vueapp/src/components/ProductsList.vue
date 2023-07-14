<template>
    <div class="post">
        <div v-if="loading" class="loading">
            Loading products list... 
        </div>

        <div v-if="productList" class="content">
            <table>
                <thead>
                    <tr>
                        <th>Product Name</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="product in productList">
                        <td>{{ product }}</td>
                    </tr>
                </tbody>
            </table>
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
                    fetch(assemblyInfoBaseUrl + 'products').then(response => response.text()),
                    fetch(assemblyInfoBaseUrl + 'services').then(response => response.text())
                ]);

                let results = await allCalls;
                this.productList = results[0].split(',');
                this.serviceList = results[1].split(',');
                this.loading = false;
                return;
            }
        },
    });
</script>