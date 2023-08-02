<script setup lang="ts">
import { computed, defineProps, ref } from 'vue';
import type {Ref} from 'vue';
import { Property, PropertyTypes } from '../lib/typeDefinitions'
import 'element-plus/es/components/message/style/css'

interface Props {
    classData: Property
}
const props = defineProps<Props>()
const currentPropertyValue: Ref<string | number> = ref("default")
const computedDepth = computed(() => props.classData.depth )
</script>

<style lang="scss">
.el-row {
  margin-bottom: 5px;
}
</style>

<template>
        <div align="left" v-if="props.classData">
            <el-row>
                <el-col :offset="computedDepth">
                    <el-text class="mx-1"><b>{{ props.classData.displayName }}</b></el-text>
                </el-col>
            </el-row>
            <el-row v-if="props.classData.propertyType === PropertyTypes.Boolean"> 
                <el-col :offset="computedDepth">
                    <el-switch 
                    v-model="currentPropertyValue"
                    size="small"
                    active-text="True"
                    inactive-text="False"
                    />
                </el-col>
            </el-row>
            <el-row v-if="props.classData.propertyType === PropertyTypes.Decimal"> 
                <el-col :offset="computedDepth">
                    <el-input v-model="currentPropertyValue" size="small" />
                </el-col>
            </el-row>
            <el-row v-if="props.classData.propertyType === PropertyTypes.Enum"> 
                <el-col :offset="computedDepth">
                    <el-select v-model="currentPropertyValue" filterable clearable class="m-2" placeholder="Choose One" size="small">
                    <el-option v-for="(enumValue, index) in props.classData.enumeratedProperties" size="small"
                                :key="index"
                                :label="enumValue"
                                :value="enumValue" />
                    </el-select> 
                </el-col>
            </el-row>
            <el-row v-if="props.classData.propertyType === PropertyTypes.Integer"> 
                <el-col :offset="computedDepth">
                    <el-input v-model="currentPropertyValue" size="small" />
                </el-col>
            </el-row>
            
            <el-row v-if="props.classData.propertyType === PropertyTypes.Object"> 
                <el-col :offset="computedDepth">
                    <el-row v-for="(classProperty, index) in props.classData.properties" :key="index">
                        <DataDisplay :classData="classProperty"/>
                    </el-row>
                </el-col>
            </el-row>

            <el-row v-if="props.classData.propertyType === PropertyTypes.String"> 
                <el-col :offset="computedDepth">
                        <el-input v-model="currentPropertyValue" placeholder="placeholder" size="small"/>
                </el-col>
            </el-row>
        </div>
</template>
