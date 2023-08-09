<script setup lang="ts">
import { ref } from 'vue';
import { JsonifyProperty } from '../lib/typeDefinitions'
import DataDisplay from './DataDisplay.vue'
import 'element-plus/es/components/message/style/css'

interface Props {
    classData: JsonifyProperty
}

const props = defineProps<Props>()
const listObjects = ref(1)
</script>

<template>
    <div>
        <p>Number of items: {{ listObjects }}</p>
    </div>
    <div>
        <el-input-number v-model="listObjects" />
        <div v-for="number in listObjects" v-bind:key="number">
            <div v-for="(classProperty, index) in props.classData.properties" :key="index">
                <DataDisplay :classData="classProperty"/>
            </div>
        </div>
    </div>
</template>