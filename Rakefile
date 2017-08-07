require 'fileutils'
gem 'albacore', '>=2.6.1'
require 'albacore'
require 'albacore/task_types/test_runner'

@current_dir = File.dirname(__FILE__)

@solution = 'AsyncAnalyzers.sln'
@build_configuration = 'Release'

@test_results_dir = 'test_results'
@xunit_console = './packages/xunit.runner.console.2.2.0\tools\xunit.console.exe'
@xunit_test_assemblies = "**/*AsyncAnalyzers.Test/bin/#{@build_configuration}/*AsyncAnalyzers.Test.dll"
@xunit_results = File.join(@current_dir, @test_results_dir, 'AsyncAnalyzers.Results.xml')

task :default => [:restore, :build_solution, :xunit_tests]

desc 'restore all nugets as per the packages.config files'
nugets_restore :restore do |p|
  p.out = 'packages'
  p.exe = 'nuget.exe'
end

desc 'Remove the bin and obj directories.'
task :clean do
    rm_rf 'AsyncAnalyzers/bin'
    rm_rf 'AsyncAnalyzers/obj'
    rm_rf 'AsyncAnalyzers.Test/bin'
    rm_rf 'AsyncAnalyzers.Test/obj'
    
    rm_rf @test_results_dir
    mkdir_p @test_results_dir
end

desc 'Clean and rebuild the solution for @build_configuration configuration.'
build :build_solution => [:clean] do |b|
    b.sln = @solution               # the solution to build
    b.target = ['Clean', 'Rebuild'] # call with an array of targets or just a single target
    b.prop 'Configuration', "#{@build_configuration}" # call with 'key, value', to specify an MsBuild property
    b.clp 'ShowEventId'             # parameters for the console logger of MsBuild
    b.logging = 'detailed'          # available verbosity levels are: q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic]. Opposite: b.be_quiet
    b.nologo                        # no Microsoft/XBuild header output
end

desc 'Run xUnit tests'
test_runner :xunit_tests do |tests|
    tests.exe = @xunit_console
    tests.files = FileList[@xunit_test_assemblies]
end