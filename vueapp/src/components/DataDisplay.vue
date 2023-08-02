<script setup lang="ts">
import { defineProps, ref } from 'vue';
import type {Ref} from 'vue';
import ListData from './ListData.vue'
import { Property, PropertyTypes } from '../lib/typeDefinitions'
import 'element-plus/es/components/message/style/css'

interface Props {
    classData: Property
}
const props = defineProps<Props>()
const currentPropertyValue: Ref<string | number> = ref("default")
</script>

<style lang="scss">
.el-row {
  margin-bottom: 5px;
}

.card-header {
  margin: 0;
  padding: 0;
}

.testDiv {
    height: 80px;
}

.text {
  font-size: 14px;
}

.item {
  margin-bottom: 18px;
}

.box-card {
  max-width: 500px;
}
</style>

<template>
    <div v-if="props.classData?.displayName">
        <el-card :body-style="{ padding: '10px' }" class="box-card">
            <template #header>
                <div align="left" class="card-header">
                    <el-text class="mx-1"><b>{{ props.classData.displayName }}</b></el-text>
                </div>
            </template>
            <div v-if="props.classData.propertyType === PropertyTypes.Boolean" align="left"> 
                    <el-switch 
                    v-model="currentPropertyValue"
                    size="small"
                    active-text="True"
                    inactive-text="False"
                    />
            </div>
            <div v-if="props.classData.propertyType === PropertyTypes.DateTime" align="left"> 
                <div class="block">
                    <el-date-picker
                        v-model="currentPropertyValue"
                        type="datetime"
                        placeholder="Select date and time"
                    />
                </div>
            </div>
            <div v-if="props.classData.propertyType === PropertyTypes.Decimal"> 
                    <el-input v-model="currentPropertyValue" size="small" />
            </div>
            <div v-if="props.classData.propertyType === PropertyTypes.Enum" align="left"> 
                    <el-select v-model="currentPropertyValue" filterable clearable class="m-2" placeholder="Choose One" size="small">
                    <el-option v-for="(enumValue, index) in props.classData.enumeratedProperties" size="small"
                                :key="index"
                                :label="enumValue"
                                :value="enumValue" />
                    </el-select> 
            </div>
            <div v-if="props.classData.propertyType === PropertyTypes.Integer"> 
                    <el-input v-model="currentPropertyValue" size="small" />
            </div>

            <div v-if="props.classData.propertyType === PropertyTypes.List">
                <ListData :classData="props.classData" />
            </div>
            
            <div v-if="props.classData.propertyType === PropertyTypes.Object"> 
                    <div v-for="(classProperty, index) in props.classData.properties" :key="index">
                        <DataDisplay :classData="classProperty"/>
                    </div>
            </div>

            <div v-if="props.classData.propertyType === PropertyTypes.String"> 
                        <el-input v-model="currentPropertyValue" placeholder="placeholder" size="small"/>
            </div>
        </el-card>
    </div>
</template>
