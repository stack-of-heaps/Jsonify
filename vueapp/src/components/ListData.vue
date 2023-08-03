<script setup lang="ts">
import { defineProps, ref } from 'vue';
import { Property } from '../lib/typeDefinitions'
import DataDisplay from './DataDisplay.vue'
import 'element-plus/es/components/message/style/css'

interface Props {
    classData: Property
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