cmake_minimum_required(VERSION 3.18)
project(CMakeRunPlatformInvoke LANGUAGES CSharp CXX)

SET(CMAKE_BUILD_TYPE "Debug")

# 指定C/C++标准
set(CMAKE_CXX_STANDARD 14)
set(CMAKE_CXX_STANDARD_REQUIRED True)
set(CMAKE_C_STANDARD 14)
set(CMAKE_C_STANDARD_REQUIRED True)

##############################################################################
# 添加动态库
add_library(NativeLib SHARED "dll/NativeLib.h" "dll/NativeLib.cpp")
target_include_directories(NativeLib PRIVATE ${CMAKE_CURRENT_SOURCE_DIR}/dll)
##############################################################################
# CS测试
add_executable(HelloWorldWithCustomedFunction   "sources/HelloWorldWithCustomedFunction.cs")
add_executable(HelloWorldWithStandardCRuntime   "sources/HelloWorldWithStandardCRuntime.cs")
add_executable(HelloWorldWithWin32              "sources/HelloWorldWithWin32.cs")

add_executable(DllImportAttributeSample         "sources/DllImportAttributeSample.cs")
add_executable(ReleaseUnmanagedMemory           "sources/ReleaseUnmanagedMemory.cs")
add_executable(HandleException                  "sources/HandleException.cs")

add_executable(MarshalString                    "sources/MarshalString.cs")
add_executable(MarshalStruct                    "sources/MarshalStruct.cs")
add_executable(MarhsalBlittableReference        "sources/MarhsalBlittableReference.cs")
##############################################################################
# 项目配置
set(target_names
    HelloWorldWithCustomedFunction
    HelloWorldWithStandardCRuntime
    HelloWorldWithWin32
    DllImportAttributeSample
    ReleaseUnmanagedMemory
    HandleException
    MarshalString
    MarshalStruct
    MarhsalBlittableReference)

foreach(target_name ${target_names})
target_compile_options(${target_name} PRIVATE "-langversion:latest")
target_compile_options(${target_name} PRIVATE "-platform:x64")
set_target_properties(${target_name} PROPERTIES VS_DOTNET_TARGET_FRAMEWORK_VERSION "v4.8")
set_target_properties(${target_name} PROPERTIES VS_DOTNET_REFERENCES "Microsoft.CSharp;
                                                                      System;
                                                                      System.Core;
                                                                      mscorlib")
endforeach()